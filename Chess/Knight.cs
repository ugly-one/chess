using System.Collections.Generic;

namespace Chess;

internal static class Knight
{
    public static List<Move> GetKnightMoves(Piece piece, Piece[] board)
    {
        var allPositions = new Vector[]
        {
            piece.Position + Vector.Up * 2 + Vector.Right,
            piece.Position + Vector.Right * 2 + Vector.Up,
            piece.Position + Vector.Right * 2 + Vector.Down,
            piece.Position + Vector.Down * 2 + Vector.Right,
            piece.Position + Vector.Down * 2 + Vector.Left,
            piece.Position + Vector.Left * 2 + Vector.Down,
            piece.Position + Vector.Up * 2 + Vector.Left,
            piece.Position + Vector.Left * 2 + Vector.Up,
        };

        var allMoves = Something.ConvertToMoves(piece, allPositions, board);
        return allMoves;
    }
}
