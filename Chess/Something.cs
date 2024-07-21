using System.Collections.Generic;
using System.Linq;

namespace Chess;

internal static class Something
{
    public static Piece? GetPieceInPosition(this Piece[] board, Vector position)
    {
        return board.FirstOrDefault(p => p.Position == position);
    }
    
    public static List<Move> ConvertToMoves(Piece piece, List<Vector> allPositions, Piece[] board)
    {
        var result = new List<Move>();

        foreach(var p in allPositions)
        {
            var pieceOnTheWay = board.GetPieceInPosition(p);
            if (pieceOnTheWay is null)
            {
                result.Add(new Move(piece, p));
            }
            else
            {
                if (pieceOnTheWay.Color != piece.Color)
                {
                    result.Add(new Capture(piece, p, pieceOnTheWay));
                }
            }
        }

        return result;
    }
    
    public static IEnumerable<Move> GetMovesInDirection(
        this Piece[] board,
        Piece piece,
        Vector step, 
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
                yield return new Capture(piece, newPos, capturedPiece);
            }
            else
            {
                yield return new Move(piece, newPos);
            }
            newPos += step;
            if (breakAfterAdding)
            {
                break;
            }
        }
    }
}