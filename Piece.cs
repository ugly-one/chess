using System;
using Godot;

namespace Chess;

public class Piece
{
	public Piece(PieceType type, Color color, Vector2 currentPosition)
	{
		Type = type;
		Color = color;
		CurrentPosition = currentPosition;
	}

	public bool Moved { get; private set; }
	public Vector2 CurrentPosition { get; private set; }
	public Color Color { get; init; }
	public PieceType Type { get; init; }

	public void Move(Vector2 newPosition)
	{
		CurrentPosition = newPosition;
		Moved = true;
	}

	public Piece CloneWith(Vector2 position)
	{
		var clonedPiece = new Piece(this.Type, this.Color, position);
		clonedPiece.Moved = this.Moved;
		return clonedPiece;
	}
}