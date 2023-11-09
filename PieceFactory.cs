using System.Collections.Generic;
using Godot;

namespace Chess;

public partial class PieceFactory : RefCounted
{
	public bool Test() => true;
	
	public (PieceUI[], King) CreatePieces(Player player, int backRow, int frontRow)
	{
		var king = new King(player, new Vector2(4, backRow));
		var pieces = new List<PieceUI>()
		{
			CreatePiece(new Rock(player, new Vector2(0, backRow)), player.GetTexture("rock")),
			CreatePiece(new Knight(player, new Vector2(1, backRow)), player.GetTexture("knight")),
			CreatePiece(new Bishop(player, new Vector2(2, backRow)), player.GetTexture("bishop")),
			CreatePiece(new Queen(player, new Vector2(3, backRow)), player.GetTexture("queen")),
			CreatePiece(king, player.GetTexture("king")),
			CreatePiece(new Bishop(player, new Vector2(5, backRow)), player.GetTexture("bishop")),
			CreatePiece(new Knight(player, new Vector2(6, backRow)), player.GetTexture("knight")),
			CreatePiece(new Rock(player, new Vector2(7, backRow)), player.GetTexture("rock"))
		};
		pieces.AddRange(CreatePawns(player, frontRow));
		return (pieces.ToArray(), king);
	}

	private List<PieceUI> CreatePawns(Player player, int frontRow)
	{
		var result = new List<PieceUI>();
		for (var i = 0; i < 8; i++)
		{
			result.Add(CreatePiece(new Pawn(player, new Vector2(i, frontRow)), player.GetTexture("pawn")));
		}
		return result;
	}

	private PieceUI CreatePiece(Piece piece, Texture2D texture)
	{
		var pieceScene = ResourceLoader.Load<PackedScene>("res://piece_csharp.tscn");
		var pieceUI = pieceScene.Instantiate<PieceUI>();
		pieceUI.Init(piece, texture);
		return pieceUI;
	}
}
