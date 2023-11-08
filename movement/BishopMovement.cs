using System.Collections.Generic;
using Godot;

namespace Bla.movement;

public partial class BishopMovement : Movement
{
	public override Texture2D GetTexture()
	{
		return base.GetTexture(Player, "bishop");
	}

	public override Vector2[] GetMoves(Piece[] pieces, Vector2 currentPosition)
	{
		var moves = new List<Vector2>();
		moves.AddRange(currentPosition.GetDirection(Vector2.Up + Vector2.Right));
		moves.AddRange(currentPosition.GetDirection(Vector2.Up + Vector2.Left));
		moves.AddRange(currentPosition.GetDirection(Vector2.Down + Vector2.Left));
		moves.AddRange(currentPosition.GetDirection(Vector2.Down + Vector2.Right));
		return moves.ToArray();
	}
}
