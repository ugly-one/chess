using System.Collections.Generic;

namespace Chess;

internal static class Knight
{
    private static Vector[] positions = new Vector[]
    {
        Vector.Up * 2 + Vector.Right,
        Vector.Right * 2 + Vector.Up,
        Vector.Right * 2 + Vector.Down,
        Vector.Down * 2 + Vector.Right,
        Vector.Down * 2 + Vector.Left,
        Vector.Left * 2 + Vector.Down,
        Vector.Up * 2 + Vector.Left,
        Vector.Left * 2 + Vector.Up,
    };
    public static IEnumerable<Move> GetKnightMoves(Piece piece, Piece[,] board)
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
        }.WithinBoard();
        return Something.ConvertToMoves(piece, allPositions, board);
    }

    public static IEnumerable<Vector> GetRay(Vector position, Piece[,] board)
    {
        foreach(var pos in positions)
        {
            var newPos = position + pos;
            var target = newPos.GetTargetInPosition(board);
            if (target != null)
                yield return target;
        }
    }
}
