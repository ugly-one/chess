using Godot;
using Chess;
using System;

namespace Chess.UI;

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
			_ => throw new NotImplementedException(),
		};
	}

	public static Texture2D GetTexture(this Color color, string piece)
	{
		var colorAsString = color == Color.WHITE ? "white" : "black";
		var path = "res://assets/" + colorAsString + "_" + piece + ".svg";
		var texture = (Texture2D)GD.Load(path);
		return texture;
	}
}
