using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Game
{
    public Board board;
    
    public Game(List<Piece> board)
    {
        this.board = new Board(board.ToArray());
    }

    /// <summary>
    /// Checks possible moves for the given piece
    /// </summary>
    /// <param name="piece">piece for which possible moves will be calculated</param>
    /// <returns></returns>
    public Move[] GetPossibleMoves(Piece piece)
    {
        var possibleMoves = board.GetMoves(piece)
            .WithinBoard();

        var possibleMovesAfterFiltering = new List<Move>();
        foreach (var possibleMove in possibleMoves)
        {
            // let's try to make the move and see if the king is still under attack
            var boardAfterMove = board.Move(piece, possibleMove);
            // find the position of the king in the new setup,
            // We can't use the member variable because the king may moved after the move we simulate the move
            var king = boardAfterMove.GetKing(piece.Color);
            // if there is still check after this move - filter the move out from possibleMoves
            var isUnderAttack = boardAfterMove.IsPieceUnderAttack(king);
            if (!isUnderAttack)
            {
                possibleMovesAfterFiltering.Add(possibleMove);
            }
        }

        if (piece.Type != PieceType.King)
        {
            return possibleMovesAfterFiltering.ToArray();
        }
        
        // add castle
        var king_ = board.GetKing(piece.Color);
        var kingUnderAttack = board.IsPieceUnderAttack(king_);

        if (kingUnderAttack)
            return possibleMovesAfterFiltering.ToArray();
        
        // short castle
        var shortCastleMove = board.TryGetCastleMove(king_, Vector2.Right, 2);
        if (shortCastleMove != null)
        {
            possibleMovesAfterFiltering.Add(shortCastleMove);
        }

        // long castle
        var longCastleMove = board.TryGetCastleMove(king_, Vector2.Left, 3);
        if (longCastleMove != null)
        {
            possibleMovesAfterFiltering.Add(longCastleMove);
        }
        
        return possibleMovesAfterFiltering.ToArray();
    }

    public Move TryMove(Piece pieceToMove, Vector2 newPosition, PieceType promotedPiece = PieceType.Queen)
    {
        var possibleMoves = GetPossibleMoves(pieceToMove);

        var move = possibleMoves.FirstOrDefault(m => m.PieceNewPosition == newPosition);
        if (move is null)
        {
            return null;
        }

        // TODO why do we have to pass 2 arguments?
        board = board.Move(move.PieceToMove, move);
        
        // did we manage to check opponent's king?
        var opponentsKing = board.GetKing(move.PieceToMove.Color.GetOppositeColor());
        var isOpponentsKingUnderFire = board.IsPieceUnderAttack(opponentsKing);
        if (!isOpponentsKingUnderFire)
        {
            // check that the opponent have a move, if not - draw
            if (GetAllPossibleMovesForColor(opponentsKing.Color).Any())
            {
                return move;
            }
            GD.Print("DRAW!!");
            return move;
        }
        // opponent's king is under fine
        GD.Print("KING IS UNDER FIRE AFTER OUR MOVE");
        // did we manage to check-mate?
        if (GetAllPossibleMovesForColor(opponentsKing.Color).Any())
        {
            return move;
        }

        GD.Print("CHECK MATE!!");
        return move;
    }

    private List<Move> GetAllPossibleMovesForColor(Color color)
    {
        var pieces = board.GetPieces(color);
        var allPossibleMoves = new List<Move>();
        foreach (var opponentsPiece in pieces)
        {
            // try to find possible moves
            var possibleMoves = GetPossibleMoves(opponentsPiece);
            allPossibleMoves.AddRange(possibleMoves);
        }

        return allPossibleMoves;
    }

   
}