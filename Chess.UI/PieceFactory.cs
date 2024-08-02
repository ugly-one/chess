using Godot;

namespace Chess.UI;

public partial class PieceFactory : RefCounted
{
	public static PieceUI CreatePiece(Vector2 position, Color color, Texture2D texture)
	{
		var pieceScene = ResourceLoader.Load<PackedScene>("res://piece_csharp.tscn");
		var pieceUI = pieceScene.Instantiate<PieceUI>();
		pieceUI.Init(position, color, texture);
		return pieceUI;
	}
}
