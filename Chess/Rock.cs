using System.Collections.Generic;

namespace Chess;

internal static class Rock
{
    private static Vector[] directions = new Vector[]
    {
        Vector.Up,
        Vector.Down,
        Vector.Left,
        Vector.Right
    };

    public static IEnumerable<Move> GetRockMoves(Piece piece, Vector position, Piece?[,] board)
    {
        foreach (var direction in directions)
        {
            foreach (var move in board.GetMovesInDirection(piece, position, direction, piece.Color))
            {
                yield return move;
            }
        }
    }

    public static IEnumerable<Vector> GetTargets(Vector position, Piece?[,] board)
    {
        foreach (var direction in directions)
        {
            var target = position.GetTargetInDirection(direction, board);
            if (target != null)
                yield return target.Value;
        }
    }
}
