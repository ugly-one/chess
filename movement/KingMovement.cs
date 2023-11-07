using System;
using Godot;

public partial class KingMovement : Movement
{
    public override bool can_move(Vector2 new_position)
    {
        if (Math.Abs(new_position.X - current_position.X) <= 1 && Math.Abs(new_position.Y - current_position.Y) <= 1)
            return true;
        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "king");
    }
}