using System.Linq;

namespace Chess.Tests;

public static class BoardExtensions
{
    public static Piece GetPiece(this Board board, PieceType pieceType)
    {
        return board.GetPieces().First(p => p.Type == pieceType);
    }

    /// <summary> return the first piece (checked in random order) that matches color and type
    public static Piece GetPiece(this Board board, Color color, PieceType pieceType)
    {
        return board.GetPieces().First(p => p.Type == pieceType && p.Color == color);
    }
}

