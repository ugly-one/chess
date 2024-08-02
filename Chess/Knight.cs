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
    public static IEnumerable<Move> GetKnightMoves(Piece piece, Vector position, Piece?[,] board)
    {
        var allPositions = new Vector[]
        {
            position + Vector.Up * 2 + Vector.Right,
            position + Vector.Right * 2 + Vector.Up,
            position + Vector.Right * 2 + Vector.Down,
            position + Vector.Down * 2 + Vector.Right,
            position + Vector.Down * 2 + Vector.Left,
            position + Vector.Left * 2 + Vector.Down,
            position + Vector.Up * 2 + Vector.Left,
            position + Vector.Left * 2 + Vector.Up,
        }.WithinBoard();
        return Something.ConvertToMoves(piece, position, allPositions, board);
    }

    public static IEnumerable<Vector> GetTargets(Vector position, Piece?[,] board)
    {
        foreach(var pos in positions)
        {
            var newPos = position + pos;
            var target = newPos.GetTargetInPosition(board);
            if (target != null)
                yield return target.Value;
        }
    }
}
