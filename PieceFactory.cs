using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public partial class PieceFactory : RefCounted
{
	public static Piece[] CreateNewGame()
	{
		var whitePieces = CreatePieces(Color.WHITE, 7, 6);
		var blackPieces= CreatePieces(Color.BLACK, 0, 1);
		return whitePieces.Concat(blackPieces).ToArray();
	}
	public static Piece[] CreatePieces(Color color, int backRow, int frontRow)
	{
		var pieces = new List<Piece>()
		{
			new Piece(PieceType.Rock, color, new Vector2(0, backRow)),
			new Piece(PieceType.Knight, color, new Vector2(1, backRow)),
			new Piece(PieceType.Bishop, color, new Vector2(2, backRow)),
			new Piece(PieceType.Queen, color, new Vector2(3, backRow)),
			new Piece(PieceType.King, color, new Vector2(4, backRow)),
			new Piece(PieceType.Bishop, color, new Vector2(5, backRow)),
			new Piece(PieceType.Knight, color, new Vector2(6, backRow)),
			new Piece(PieceType.Rock, color, new Vector2(7, backRow))
		};
		pieces.AddRange(CreatePawns(color, frontRow));
		return pieces.ToArray();
	}
	
	private static List<Piece> CreatePawns(Color color, int frontRow)
	{
		var result = new List<Piece>();
		for (var i = 0; i < 8; i++)
		{
			result.Add(new Piece(PieceType.Pawn, color, new Vector2(i, frontRow)));
		}
		return result;
	}

	public static PieceUI CreatePiece(Vector2 position, Color color, Texture2D texture)
	{
		var pieceScene = ResourceLoader.Load<PackedScene>("res://piece_csharp.tscn");
		var pieceUI = pieceScene.Instantiate<PieceUI>();
		pieceUI.Init(position, color, texture);
		return pieceUI;
	}
}
