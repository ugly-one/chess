using System.Collections.Generic;
using System.Linq;

namespace Chess;

public static class Something
{
    public static Piece GetPieceInPosition(this Piece[] board, Vector position)
    {
        return board.First(p => p.Position == position);
    }
    
    public static Move[] ConvertToMoves(Piece piece, List<Vector> allPositions, Piece[] board)
    {
        var result = new List<Move>();

        foreach(var p in allPositions)
        {
            var pieceOnTheWay = board.GetPieceInPosition(p);
            if (pieceOnTheWay is null)
            {
                result.Add(Move.RegularMove(piece, p));
            }
            else
            {
                if (pieceOnTheWay.Color != piece.Color)
                {
                    result.Add(Move.Capture(piece, p, pieceOnTheWay));
                }
            }
        }
        return result.ToArray();
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