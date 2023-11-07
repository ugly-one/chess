using Godot;

public partial class RockMovement : Movement
{
    public override bool can_move(Vector2 new_position)
    {
        if (current_position.Y == new_position.Y)
            return true;
        if (new_position.X == current_position.X)
            return true;
        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "rock");
    }
}