using System.Collections.Generic;

namespace Chess;

internal static class Bishop
{
    private static Vector[] directions = new Vector[]
    {
        Vector.Up + Vector.Right,
        Vector.Up + Vector.Left,
        Vector.Down + Vector.Right,
        Vector.Down + Vector.Left,
    };

    public static IEnumerable<Move> GetBishopMoves(Piece piece, Piece[,] board)
    {
        foreach(var direction in directions)
        {
            foreach(var move in board.GetMovesInDirection(piece, direction, piece.Color))
            {
                yield return move;
            }
        }
    }
}
