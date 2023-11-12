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
            // let's try to make the move and see if the king is under attack, if yes, move is not allowed
            var boardAfterMove = board.Move(possibleMove);
            var king = boardAfterMove.GetKing(piece.Color);
            if (!boardAfterMove.IsPieceUnderAttack(king))
            {
                possibleMovesAfterFiltering.Add(possibleMove);
            }
        }

        if (piece.Type != PieceType.King)
        {
            return possibleMovesAfterFiltering.ToArray();
        }
        
        // TODO it's a bit odd that we have to add castling here. 
        // In theory we should be able to put it in GetMoves method (the one handling the king)
        // But the reason is that we need to call IsPieceUnderAttack and it calls GetMoves on all pieces.
        // And then we will end-up in a stack overflow where GetMoves calls GetMoves and so on.
        // Solution. As with other pieces: King should spit out castling as a possible move and then we should filter it out
        // as we do filtering out here.
        // Question: why do we have to do filtering out? Why the pieces are not smart enough to check if the move that they produce is actually not possible.
        // Maybe it's fine. The initial idea was:
        // let a piece spit out all possible moves, in general - regardless what state of the board we have and then filter the moves out.
        // bishopMoves = GetBishopMoves(position) <- get all possible moves based on the position
        // bishopMoves = GetBishopMoves(bishopMoves, board) <- filter out invalid moves
        // I'm not sure if helps with anything. I think the problem is that IsPieceUnderAttack ends up in stack overflow
        
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

        board = board.Move(move);
        
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