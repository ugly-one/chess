using System.Collections.Generic;
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
		var moves = new List<Vector2>();
		AddDirection(currentPosition, Vector2.Up + Vector2.Right, moves);
		AddDirection(currentPosition, Vector2.Up + Vector2.Left, moves);
		AddDirection(currentPosition, Vector2.Down + Vector2.Left, moves);
		AddDirection(currentPosition, Vector2.Down + Vector2.Right, moves);
		return moves.ToArray();
	}

	private static void AddDirection(Vector2 currentPosition, Vector2 step, List<Vector2> moves)
	{
		var newPos = currentPosition + step;
		while (newPos.IsWithinBoard())
		{
			moves.Add(newPos);
			newPos += step;
		}
	}
}
