using System.Collections.Generic;
using Godot;

namespace Bla;

public partial class PieceFactory : RefCounted
{
	public bool Test() => true;
	
	public Piece[] CreatePieces(Player player, int backRow, int frontRow)
	{
		var pieces = new List<Piece>()
		{
			CreatePiece(player, new RockMovement(), new Vector2(0, backRow)),
			CreatePiece(player, new KnightMovement(), new Vector2(1, backRow)),
			CreatePiece(player, new BishopMovement(), new Vector2(2, backRow)),
			CreatePiece(player, new KingMovement(), new Vector2(3, backRow)),
			CreatePiece(player, new QueenMovement(), new Vector2(4, backRow)),
			CreatePiece(player, new BishopMovement(), new Vector2(5, backRow)),
			CreatePiece(player, new KnightMovement(), new Vector2(6, backRow)),
			CreatePiece(player, new RockMovement(), new Vector2(7, backRow)),
		};
		pieces.AddRange(CreatePawns(player, frontRow));
		return pieces.ToArray();
	}

	private List<Piece> CreatePawns(Player player, int frontRow)
	{
		var result = new List<Piece>();
		for (var i = 0; i < 8; i++)
		{
			result.Add(CreatePiece(player, new PawnMovement(), new Vector2(i, frontRow)));
		}
		return result;
	}

	private Piece CreatePiece(Player player, Movement movement, Vector2 position)
	{
		movement.Player = player;
		movement.current_position = position;
		var pieceScene = ResourceLoader.Load<PackedScene>("res://piece_csharp.tscn");
		var piece = pieceScene.Instantiate<Piece>();
		piece.Init(movement);
		return piece;
	}
}
