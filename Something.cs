using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public static class Something
{
    public static Piece GetPieceInPosition(this Piece[] board, Vector2 position)
    {
        return board.FirstOrDefault(p => p.Position == position);
    }
    
    public static Move[] ConvertToMoves(Piece piece, List<Vector2> allPositions, Piece[] board)
    {
        return allPositions.Select(p =>
        {
            var pieceOnTheWay = board.GetPieceInPosition(p);
            if (pieceOnTheWay is null)
            {
                return Move.RegularMove(piece, p);
            }
            if (pieceOnTheWay.Color != piece.Color)
            {
                return Move.Capture(piece, p, pieceOnTheWay);
            }
            return null;
        }).Where(m => m != null).ToArray();
    }
    
    public static IEnumerable<Move> GetMovesInDirection(
        this Piece[] board,
        Piece piece,
        Vector2 step, 
        Color color)
    {
        var newPos = piece.Position + step;
        var breakAfterAdding = false;
        while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(color, board))
        {
            // we should pass only opponents pieces to GetPieceInPosition
            var capturedPiece = board.GetPieceInPosition(newPos);
            if (capturedPiece != null)
            {
                breakAfterAdding = true;
                yield return Move.Capture(piece, newPos, capturedPiece);
                
            }
            else
            {
                yield return Move.RegularMove(piece, newPos);
            }
            newPos += step;
            if (breakAfterAdding)
            {
                break;
            }
        }
    }
}