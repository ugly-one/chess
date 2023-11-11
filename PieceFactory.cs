using System.Collections.Generic;
using Godot;

namespace Chess;

public partial class PieceFactory : RefCounted
{
	public PieceUI[] CreatePieces(Color color, int backRow, int frontRow)
	{
		var king = new Piece(PieceType.King, color, new Vector2(4, backRow));
		var pieces = new List<PieceUI>()
		{
			CreatePiece(new Piece(PieceType.Rock, color, new Vector2(0, backRow)), color.GetTexture("rock")),
			CreatePiece(new Piece(PieceType.Knight, color, new Vector2(1, backRow)), color.GetTexture("knight")),
			CreatePiece(new Piece(PieceType.Bishop, color, new Vector2(2, backRow)), color.GetTexture("bishop")),
			CreatePiece(new Piece(PieceType.Queen, color, new Vector2(3, backRow)), color.GetTexture("queen")),
			CreatePiece(king, color.GetTexture("king")),
			CreatePiece(new Piece(PieceType.Bishop, color, new Vector2(5, backRow)), color.GetTexture("bishop")),
			CreatePiece(new Piece(PieceType.Knight, color, new Vector2(6, backRow)), color.GetTexture("knight")),
			CreatePiece(new Piece(PieceType.Rock, color, new Vector2(7, backRow)), color.GetTexture("rock"))
		};
		pieces.AddRange(CreatePawns(color, frontRow));
		return pieces.ToArray();
	}
	
	private List<PieceUI> CreatePawns(Color color, int frontRow)
	{
		var result = new List<PieceUI>();
		for (var i = 0; i < 8; i++)
		{
			result.Add(CreatePiece(new Piece(PieceType.Pawn, color, new Vector2(i, frontRow)), color.GetTexture("pawn")));
		}
		return result;
	}

	public PieceUI CreatePiece(Piece piece, Texture2D texture)
	{
		var pieceScene = ResourceLoader.Load<PackedScene>("res://piece_csharp.tscn");
		var pieceUI = pieceScene.Instantiate<PieceUI>();
		pieceUI.Init(piece, texture);
		return pieceUI;
	}
}
