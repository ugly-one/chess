using System;
using Godot;

namespace Bla.movement;

public partial class KingMovement : Movement
{
    public override bool CanMove(Vector2 newPosition)
    {
        return Math.Abs(newPosition.X - CurrentPosition.X) <= 1 && Math.Abs(newPosition.Y - CurrentPosition.Y) <= 1;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "king");
    }
}