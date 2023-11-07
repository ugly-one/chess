using Godot;

public abstract partial class Movement : Node
{
	public bool Moved;
	public Vector2 current_position;
	public Player Player;

	public abstract bool can_move(Vector2 new_position);

	// TODO this should be a static method outside of Movement class
	internal Texture2D GetTexture(Player player, string piece)
	{
		var color = player == Player.WHITE ? "white" : "black";
		var image = Image.LoadFromFile("res://assets/" + color + "_" + piece + ".svg");
		return ImageTexture.CreateFromImage(image);
	}

	public abstract Texture2D GetTexture();
}
