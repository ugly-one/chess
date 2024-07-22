using System.Collections.Generic;

namespace Chess;

internal static class Something
{
    public static List<Move> ConvertToMoves(Piece piece, ICollection<Vector> allPositions, Piece[,] board)
    {
        var result = new List<Move>(allPositions.Count);

        foreach (var p in allPositions)
        {
            if (!p.IsWithinBoard())
            {
                continue;
            }
            var pieceOnTheWay = board[p.X, p.Y];
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
            this Piece[,] board,
            Piece piece,
            Vector step,
            Color color)
    {
        var newPos = piece.Position + step;
        var breakAfterAdding = false;
        while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(color, board))
        {
            // we should pass only opponents pieces to GetPieceInPosition
            var capturedPiece = board[newPos.X, newPos.Y];
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
