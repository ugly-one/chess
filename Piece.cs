using Godot;

namespace Chess;

public record Piece(PieceType Type, bool Moved, Player Player, Vector2 CurrentPosition)
{
	public bool Moved { get; set; } = Moved;
	public Vector2 CurrentPosition { get; set; } = CurrentPosition;
}