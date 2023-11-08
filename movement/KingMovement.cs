using System;
using System.Collections.Generic;
using Godot;

namespace Bla.movement;

public partial class KingMovement : Movement
{
    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "king");
    }

    public override Vector2[] GetMoves(Movement[] pieces)
    {
        // TODO king can step on its own pieces
        return new List<Vector2>()
        {
            CurrentPosition + Vector2.Up,
            CurrentPosition + Vector2.Down,
            CurrentPosition + Vector2.Left,
            CurrentPosition + Vector2.Right,
            CurrentPosition + Vector2.Up + Vector2.Right,
            CurrentPosition + Vector2.Up + Vector2.Left,
            CurrentPosition + Vector2.Down + Vector2.Right,
            CurrentPosition + Vector2.Down + Vector2.Left,
        }.ToArray();
    }
}