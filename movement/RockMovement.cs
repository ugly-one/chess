using Godot;

namespace Bla.movement;

public partial class RockMovement : Movement
{
    public override bool CanMove(Vector2 newPosition)
    {
        if (CurrentPosition.Y == newPosition.Y)
            return true;
        if (newPosition.X == CurrentPosition.X)
            return true;
        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "rock");
    }
}