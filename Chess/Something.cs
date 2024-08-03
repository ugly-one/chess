using System.Collections.Generic;

namespace Chess;

internal static class Something
{
    public static IEnumerable<Move> ConvertToMoves(Piece piece, Vector currentPosition, IEnumerable<Vector> positions, Piece?[,] board)
    {
        foreach (var position in positions)
        {
            var pieceOnTheWay = board[position.X, position.Y];
            if (pieceOnTheWay is null)
            {
                yield return new Move(piece, currentPosition, position);
            }
            else
            {
                if (pieceOnTheWay.Value.Color != piece.Color)
                {
                    yield return new Move(piece, currentPosition, position);
                }
            }
        }
    }

    public static IEnumerable<Move> GetMovesInDirection(
            this Piece?[,] board,
            Piece piece,
            Vector currentPosition,
            Vector step,
            Color color)
    {
        var newPos = currentPosition + step;
        var breakAfterAdding = false;
        while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(color, board))
        {
            // we should pass only opponents pieces to GetPieceInPosition
            var capturedPiece = board[newPos.X, newPos.Y];
            if (capturedPiece != null)
            {
                breakAfterAdding = true;
                yield return new Move(piece, currentPosition, newPos);
            }
            else
            {
                yield return new Move(piece, currentPosition, newPos);
            }
            newPos += step;
            if (breakAfterAdding)
            {
                break;
            }
        }
    }
    
    public static Piece? GetTargetPieceInDirection(
            this Vector position,
            Vector direction,
            Piece?[,] board)
    {
        var newPos = position + direction;
        while (newPos.IsWithinBoard())
        {
            var pieceAtNewPosition = board[newPos.X, newPos.Y];
            if (pieceAtNewPosition != null)
            {
                return pieceAtNewPosition;
            }
            newPos += direction;
        }
        return null;
    }

    public static Vector? GetTargetInPosition(
        this Vector position,
        Piece?[,] board)
    {
        if (!position.IsWithinBoard())
        {
            return null;
        }
        var pieceAtPosition = board[position.X, position.Y];
        return pieceAtPosition == null ? null : position;
    }
}
