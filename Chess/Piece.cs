using System;

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

    public override string ToString()
    {
        return (Type, Color) switch
        {
            (PieceType.King, Color.WHITE) => "\u2654",
            (PieceType.King, Color.BLACK) => "\u265a",
            (PieceType.Queen, Color.WHITE) => "\u2655",
            (PieceType.Queen, Color.BLACK) => "\u265b",
            (PieceType.Rock, Color.WHITE) => "\u2656",
            (PieceType.Rock, Color.BLACK) => "\u265c",
            (PieceType.Bishop, Color.WHITE) => "\u2657",
            (PieceType.Bishop, Color.BLACK) => "\u265d",
            (PieceType.Knight, Color.WHITE) => "\u2658",
            (PieceType.Knight, Color.BLACK) => "\u265e",
            (PieceType.Pawn, Color.WHITE) => "\u2659",
            (PieceType.Pawn, Color.BLACK) => "\u265f",
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
}
