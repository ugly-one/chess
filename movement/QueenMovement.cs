using Godot;

public partial class QueenMovement : Movement
{
    public override bool can_move(Vector2 new_position)
    {
        var move_vector = (new_position - current_position).Abs();
        if (move_vector.X == move_vector.Y)
            return true;
        if (current_position.Y == new_position.Y)
            return true;
        if (new_position.X == current_position.X)
            return true;
        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "queen");
    }
}