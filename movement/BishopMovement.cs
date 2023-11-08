using System.Collections.Generic;
using Godot;

namespace Bla.movement;

public partial class BishopMovement : Movement
{
	public override Texture2D GetTexture()
	{
		return base.GetTexture(Player, "bishop");
	}

	public override Vector2[] GetMoves(Movement[] pieces)
	{
		var moves = new List<Vector2>();
		moves.AddRange(CurrentPosition.GetDirection(Vector2.Up + Vector2.Right, pieces, Player));
		moves.AddRange(CurrentPosition.GetDirection(Vector2.Up + Vector2.Left, pieces, Player));
		moves.AddRange(CurrentPosition.GetDirection(Vector2.Down + Vector2.Left, pieces, Player));
		moves.AddRange(CurrentPosition.GetDirection(Vector2.Down + Vector2.Right, pieces, Player));
		return moves.ToArray();
	}
}
