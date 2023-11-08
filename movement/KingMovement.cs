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

    public override Vector2[] GetMoves(Piece[] pieces, Vector2 currentPosition)
    {
        return new List<Vector2>()
        {
            currentPosition + Vector2.Up,
            currentPosition + Vector2.Down,
            currentPosition + Vector2.Left,
            currentPosition + Vector2.Right,
            currentPosition + Vector2.Up + Vector2.Right,
            currentPosition + Vector2.Up + Vector2.Left,
            currentPosition + Vector2.Down + Vector2.Right,
            currentPosition + Vector2.Down + Vector2.Left,
        }.ToArray();
    }
}