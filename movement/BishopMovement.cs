using Godot;

namespace Bla.movement;

public partial class BishopMovement : Movement
{
	public override bool CanMove(Vector2 newPosition)
	{
		var moveVector = (newPosition - CurrentPosition).Abs();
		if (moveVector.X == moveVector.Y){
			return true;
		}
		return false;
	}

	public override Texture2D GetTexture()
	{
		return base.GetTexture(Player, "bishop");
	}

	public override Vector2[] GetMoves(Piece[] pieces, Vector2 currentPosition)
	{
		throw new System.NotImplementedException();
	}
}
