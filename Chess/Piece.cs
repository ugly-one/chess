using System;

namespace Chess;

public record Piece(PieceType Type, Color Color, Vector Position, bool Moved = false)
{
	public Piece Move(Vector position)
	{
		var clonedPiece = new Piece(this.Type, this.Color, position, true);
		return clonedPiece;
	}

	public override string ToString()
	{
		return (Type, Color) switch
		{
            // For some reason I have to switch colors and give white pieces black characters
			(PieceType.King, Color.BLACK) => "\u2654",
			(PieceType.King, Color.WHITE) => "\u265a",
			(PieceType.Queen, Color.BLACK) => "\u2655",
			(PieceType.Queen, Color.WHITE) => "\u265b",
			(PieceType.Rock, Color.BLACK) => "\u2656",
			(PieceType.Rock, Color.WHITE) => "\u265c",
			(PieceType.Bishop, Color.BLACK) => "\u2657",
			(PieceType.Bishop, Color.WHITE) => "\u265d",
			(PieceType.Knight, Color.BLACK) => "\u2658",
			(PieceType.Knight, Color.WHITE) => "\u265e",
			(PieceType.Pawn, Color.BLACK) => "\u2659",
			(PieceType.Pawn, Color.WHITE) => "\u265f",
			_ => throw new ArgumentOutOfRangeException(),
		};
	}
}
