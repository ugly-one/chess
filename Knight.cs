using System.Collections.Generic;
using Godot;

namespace Chess;

public static class Knight
{
    public static Move[] GetKnightMoves(Piece piece, Piece[] board)
    {
        var allPositions = new List<Vector2>()
        {
            piece.Position + Vector2.Up * 2 + Vector2.Right,
            piece.Position + Vector2.Right * 2 + Vector2.Up,
            piece.Position + Vector2.Right * 2 + Vector2.Down,
            piece.Position + Vector2.Down * 2 + Vector2.Right,
            piece.Position + Vector2.Down * 2 + Vector2.Left,
            piece.Position + Vector2.Left * 2 + Vector2.Down,
            piece.Position + Vector2.Up * 2 + Vector2.Left,
            piece.Position + Vector2.Left * 2 + Vector2.Up,
        };

        var allMoves = Something.ConvertToMoves(piece, allPositions, board);
        return allMoves;
    }
}