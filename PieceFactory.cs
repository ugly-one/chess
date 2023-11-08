using System.Collections.Generic;
using Godot;

namespace Bla;

public partial class PieceFactory : RefCounted
{
	public bool Test() => true;
	
	public (PieceUI[], Vector2) CreatePieces(Player player, int backRow, int frontRow)
	{
		var kingPlacement = new Vector2(4, backRow);
		var pieces = new List<PieceUI>()
		{
			CreatePiece(player, new movement.Rock(), new Vector2(0, backRow)),
			CreatePiece(player, new movement.Knight(), new Vector2(1, backRow)),
			CreatePiece(player, new movement.Bishop(), new Vector2(2, backRow)),
			CreatePiece(player, new movement.Queen(), new Vector2(3, backRow)),
			CreatePiece(player, new movement.King(), kingPlacement),
			CreatePiece(player, new movement.Bishop(), new Vector2(5, backRow)),
			CreatePiece(player, new movement.Knight(), new Vector2(6, backRow)),
			CreatePiece(player, new movement.Rock(), new Vector2(7, backRow)),
		};
		pieces.AddRange(CreatePawns(player, frontRow));
		return (pieces.ToArray(), kingPlacement);
	}

	private List<PieceUI> CreatePawns(Player player, int frontRow)
	{
		var result = new List<PieceUI>();
		for (var i = 0; i < 8; i++)
		{
			result.Add(CreatePiece(player, new movement.Pawn(), new Vector2(i, frontRow)));
		}
		return result;
	}

	private PieceUI CreatePiece(Player player, movement.Piece piece, Vector2 position)
	{
		piece.Player = player;
		piece.CurrentPosition = position;
		var pieceScene = ResourceLoader.Load<PackedScene>("res://piece_csharp.tscn");
		var pieceUI = pieceScene.Instantiate<PieceUI>();
		pieceUI.Init(piece);
		return pieceUI;
	}
}
