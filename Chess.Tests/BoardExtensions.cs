using System.Linq;

namespace Chess.Tests;

public static class BoardExtensions
{
    public static (Piece, Vector) GetPiece(this Board board, PieceType pieceType)
    {
        return board.GetPieces().First(p => p.Item1.Type == pieceType);
    }

    public static Vector GetPosition(this Board board, PieceType pieceType)
    {
        return board.GetPieces().First(p => p.Item1.Type == pieceType).Item2;
    }


    /// <summary> return the first piece (checked in random order) that matches color and type
    public static (Piece, Vector) GetPiece(this Board board, Color color, PieceType pieceType)
    {
        return board.GetPieces().First(p => p.Item1.Type == pieceType && p.Item1.Color == color);
    }

    public static Vector GetPosition(this Board board, Color color, PieceType pieceType)
    {
        return board.GetPieces().First(p => p.Item1.Type == pieceType && p.Item1.Color == color).Item2;
    }
}

