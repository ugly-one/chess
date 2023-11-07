using Godot;
using System;

public partial class BishopMovement : Movement
{
	public override bool can_move(Vector2 new_position)
	{
		var move_vector = (new_position - current_position).Abs();
		if (move_vector.X == move_vector.Y){
			return true;
		}
		return false;
	}

	public override Texture2D GetTexture()
	{
		return base.GetTexture(Player, "bishop");
	}
}
