using System.Collections.Generic;
using Godot;

public partial class Bishop : Piece
{
	public override Texture2D GetTexture()
	{
		return Player.GetTexture("bishop");
	}

	public override Vector2[] GetMoves(Piece[] pieces)
	{
		var moves = new List<Vector2>();
		moves.AddRange(CurrentPosition.GetDirection(Vector2.Up + Vector2.Right, pieces, Player));
		moves.AddRange(CurrentPosition.GetDirection(Vector2.Up + Vector2.Left, pieces, Player));
		moves.AddRange(CurrentPosition.GetDirection(Vector2.Down + Vector2.Left, pieces, Player));
		moves.AddRange(CurrentPosition.GetDirection(Vector2.Down + Vector2.Right, pieces, Player));
		return moves.ToArray();
	}

	public Bishop(Player player, Vector2 position) : base(player, position)
	{
	}
}
