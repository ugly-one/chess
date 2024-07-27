using System.Collections.Generic;

namespace Chess;

internal static class Something
{
    public static IEnumerable<Move> ConvertToMoves(Piece piece, ICollection<Vector> positions, Piece[,] board)
    {
        foreach (var position in positions)
        {
            if (!position.IsWithinBoard())
            {
                continue;
            }
            var pieceOnTheWay = board[position.X, position.Y];
            if (pieceOnTheWay is null)
            {
                yield return new Move(piece, position);
            }
            else
            {
                if (pieceOnTheWay.Color != piece.Color)
                {
                    yield return new Capture(piece, position, pieceOnTheWay);
                }
            }
        }
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
