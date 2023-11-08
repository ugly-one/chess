using System;
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
        throw new NotImplementedException();
    }
}