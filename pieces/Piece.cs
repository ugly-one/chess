using Godot;

public abstract partial class Piece : Node
{
	public bool Moved;
	public Vector2 CurrentPosition;
	public Player Player;
	
	// TODO this should be a static method outside of Movement class
	internal Texture2D GetTexture(Player player, string piece)
	{
		var color = player == Player.WHITE ? "white" : "black";
		var image = Image.LoadFromFile("res://assets/" + color + "_" + piece + ".svg");
		return ImageTexture.CreateFromImage(image);
	}

	public abstract Texture2D GetTexture();

	public abstract Vector2[] GetMoves(Piece[] pieces);
}
