using System;
using System.Collections.Generic;
using Godot;

namespace Bla.movement;

public partial class KnightMovement : Movement
{
    public override bool CanMove(Vector2 newPosition)
    {
        if (Math.Abs(newPosition.X - CurrentPosition.X) == 1 && Math.Abs(newPosition.Y - CurrentPosition.Y) == 2)
            return true;
        if (Math.Abs(newPosition.X - CurrentPosition.X) == 2 && Math.Abs(newPosition.Y - CurrentPosition.Y) == 1)
            return true;
        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "knight");
    }

    public override Vector2[] GetMoves(Piece[] pieces, Vector2 currentPosition)
    {
        var moves = new List<Vector2>();
        moves.Add(currentPosition + Vector2.Up * 2 + Vector2.Right);
        moves.Add(currentPosition + Vector2.Right * 2 + Vector2.Up);
        moves.Add(currentPosition + Vector2.Right * 2 + Vector2.Down);
        moves.Add(currentPosition + Vector2.Down * 2 + Vector2.Right);
        moves.Add(currentPosition + Vector2.Down * 2 + Vector2.Left);
        moves.Add(currentPosition + Vector2.Left * 2 + Vector2.Down);
        moves.Add(currentPosition + Vector2.Up * 2 + Vector2.Left);
        moves.Add(currentPosition + Vector2.Left * 2 + Vector2.Up);
        return moves.ToArray();
    }
}