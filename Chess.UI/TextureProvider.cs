using Godot;
using Chess;

public static class TextureProvider
{
	public static Texture2D GetTexture(this Piece piece)
	{
		var color = piece.Color;
		return piece.Type switch
		{
			PieceType.Bishop => color.GetTexture("bishop"),
			PieceType.King => color.GetTexture("king"),
			PieceType.Queen => color.GetTexture("queen"),
			PieceType.Rock => color.GetTexture("rock"),
			PieceType.Pawn => color.GetTexture("pawn"),
			PieceType.Knight => color.GetTexture("knight"),
		};
	}
	/// <summary>
	/// this shouldn't be an extension method on player, but maybe we should have 2 methods GetWhiteTexture and GetBlackTexture
	/// that would ease the usage of it outside of PieceFactory class
	/// </summary>
	/// <param name="colorr"></param>
	/// <param name="piece"></param>
	/// <returns></returns>
	public static Texture2D GetTexture(this Color color, string piece)
	{
		var colorAsString = color == Color.WHITE ? "white" : "black";
		var path = "res://assets/" + colorAsString + "_" + piece + ".svg";
		var texture = (Texture2D)GD.Load(path);
		return texture;
	}
}
