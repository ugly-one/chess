using Godot;

namespace Bla.movement;

public partial class PawnMovement : Movement
{
    public override bool CanMove(Vector2 newPosition)
    {
        if (CurrentPosition.Y == newPosition.Y)
            return false;
        if (newPosition.X != CurrentPosition.X)
            return false;

        if (Player == Player.WHITE)
        {
            if (newPosition.Y - 1 == CurrentPosition.Y || (!Moved && newPosition.Y - 2 == CurrentPosition.Y))
                return true;
            return false;
        }

        if (newPosition.Y + 1 == CurrentPosition.Y || (!Moved && newPosition.Y + 2 == CurrentPosition.Y))
            return true;
        
        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "pawn");
    }
}