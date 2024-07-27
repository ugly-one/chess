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

    public static IEnumerable<Move> GetRockMoves(Piece piece, Piece[,] board)
    {
        foreach (var direction in directions)
        {
            foreach (var move in board.GetMovesInDirection(piece, direction, piece.Color))
            {
                yield return move;
            }
        }
    }
}
