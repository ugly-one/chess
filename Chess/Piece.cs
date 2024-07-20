namespace Chess;

public record Piece(PieceType Type, Color Color, Vector Position, bool Moved = false)
{
	public Piece Move(Vector position)
	{
		var clonedPiece = new Piece(this.Type, this.Color, position, true);
		return clonedPiece;
	}
}