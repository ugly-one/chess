using System;
using Godot;

public abstract partial class Piece : Node
{
	public bool Moved { get; set; }
	public Vector2 CurrentPosition { get; set; }
	public Player Player { get; }

	public Piece(Player player, Vector2 position)
	{
		Player = player;
		CurrentPosition = position;
	}

	public abstract Vector2[] GetMoves(Piece[] pieces);

	public abstract Piece Copy();
}
