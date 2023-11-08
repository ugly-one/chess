using Godot;

public partial class PawnMovement : Movement
{
    public override bool can_move(Vector2 new_position)
    {
        if (current_position.Y == new_position.Y)
            return false;
        if (new_position.X != current_position.X)
            return false;

        // TODO add support for taking opponents pieces
        if (Player == Player.WHITE)
        {
            if (new_position.Y - 1 == current_position.Y || (!Moved && new_position.Y - 2 == current_position.Y))
                return true;
            return false;
        }

        if (new_position.Y + 1 == current_position.Y || (!Moved && new_position.Y + 2 == current_position.Y))
            return true;
        
        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "pawn");
    }
}