using System;
using System.Collections.Generic;
using Godot;

namespace Bla.movement;

public partial class KnightMovement : Movement
{
    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "knight");
    }

    public override Vector2[] GetMoves(Movement[] pieces)
    {
        // TODO king can step on its own pieces
        var moves = new List<Vector2>();
        moves.Add(CurrentPosition + Vector2.Up * 2 + Vector2.Right);
        moves.Add(CurrentPosition + Vector2.Right * 2 + Vector2.Up);
        moves.Add(CurrentPosition + Vector2.Right * 2 + Vector2.Down);
        moves.Add(CurrentPosition + Vector2.Down * 2 + Vector2.Right);
        moves.Add(CurrentPosition + Vector2.Down * 2 + Vector2.Left);
        moves.Add(CurrentPosition + Vector2.Left * 2 + Vector2.Down);
        moves.Add(CurrentPosition + Vector2.Up * 2 + Vector2.Left);
        moves.Add(CurrentPosition + Vector2.Left * 2 + Vector2.Up);
        return moves.ToArray();
    }
}