using System;
using Godot;

namespace Chess;

public class Piece
{
	public Piece(PieceType type, Player player, Vector2 currentPosition)
	{
		Type = type;
		Player = player;
		CurrentPosition = currentPosition;
	}

	public bool Moved { get; private set; }
	public Vector2 CurrentPosition { get; private set; }
	public Player Player { get; init; }
	public PieceType Type { get; init; }

	public EventHandler<Vector2> MovedEvent;
	public void Move(Vector2 newPosition)
	{
		CurrentPosition = newPosition;
		Moved = true;
		MovedEvent.Invoke(this, newPosition);
	}

	public Piece CloneWith(Vector2 position)
	{
		var clonedPiece = new Piece(this.Type, this.Player, position);
		clonedPiece.Moved = this.Moved;
		return clonedPiece;
	}
}