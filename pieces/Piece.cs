using Godot;

public abstract partial class Piece : Node
{
	public bool Moved;
	public Vector2 CurrentPosition { get; set; }
	public Player Player { get; }

	public Piece(Player player, Vector2 position)
	{
		Player = player;
		CurrentPosition = position;
	}

	public abstract Texture2D GetTexture();

	public abstract Vector2[] GetMoves(Piece[] pieces);
}
