using System.Collections.Generic;

namespace Chess;

public static class Queen
{
    private static Vector[] directions = new Vector[]
    {
        Vector.Up + Vector.Right,
        Vector.Up + Vector.Left,
        Vector.Down + Vector.Right,
        Vector.Down + Vector.Left,
        Vector.Up,
        Vector.Down,
        Vector.Left,
        Vector.Right
    };

    public static IEnumerable<Move> GetQueenMoves(Piece piece, Vector position, Piece?[,] board)
    {
        foreach(var direction in directions)
        {
            foreach(var move in board.GetMovesInDirection(piece, position, direction, piece.Color))
            {
                yield return move;
            }
        }
    }
}
