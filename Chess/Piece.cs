namespace Chess;

public record Piece
{
	public Piece(PieceType type, Color color, Vector position, bool moved = false)
	{
		Type = type;
		Color = color;
		Position = position;
		Moved = moved;
	}

	public bool Moved { get; }
	public Vector Position { get; }
	public Color Color { get; }
	public PieceType Type { get; }

	public Piece Move(Vector position)
	{
		var clonedPiece = new Piece(this.Type, this.Color, position, true);
		return clonedPiece;
	}
}