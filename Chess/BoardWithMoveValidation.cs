using System.Collections.Generic;
using System.Linq;

namespace Chess;

public class BoardWithMoveValidation
{
    private readonly Board board;
    private HashSet<Move> movesCache;

    public BoardWithMoveValidation(Board board)
    {
        this.board = board;
        this.movesCache = new HashSet<Move>();
    }
    public IEnumerable<Move> GetAllPossibleMoves()
    {
        var moves = board.GetAllPossibleMoves();
        movesCache = new HashSet<Move>(moves);
        return moves;
    }

    public (bool, BoardWithMoveValidation) TryMove(Move move, PieceType? promotedPiece = null)
    {
        if (!movesCache.Contains(move))
        {
            var possibleMoves = board.GetPossibleMoves(move.PieceToMove);
            foreach (var moveToAdd in possibleMoves)
            {
                movesCache.Add(moveToAdd);
            }
            var existingMove = possibleMoves.FirstOrDefault(m => m.PieceNewPosition == move.PieceNewPosition);
            if (move == null)
            {
                return (false, this);
            }
        }

        var (success, newBoard) = board.TryMove(move, promotedPiece);

        return (success, new BoardWithMoveValidation(newBoard));
    }
}

