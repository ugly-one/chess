using System;
using System.Collections.Generic;
using System.Linq;
using Chess;


public static class OtherExtensions
{
    public static IEnumerable<Move> GetPossibleMoves(this Board board, Vector field)
    {
        return board.GetAllPossibleMoves().Where(m => m.PieceOldPosition == field);
    }

    public static string ToChessNotation(this Vector position)
    {
        var number = 8 - position.Y;
        if (number < 1 || number > 8)
        {
            throw new NotSupportedException();
        }
        var letter = position.X switch
        {
            0 => "a",
            1 => "b",
            2 => "c",
            3 => "d",
            4 => "e",
            5 => "f",
            6 => "g",
            7 => "h",
            _ => throw new NotSupportedException()
        };
        return letter + number;
    }

    public static Color GetOpposite(this Color color)
    {
        return color == Color.BLACK ? Color.WHITE : Color.BLACK;
    }
}

internal static class Extentions
{
    //public static bool IsOccupiedBy(this Vector position, Color color, IEnumerable<Piece> allPieces)
    //{
    //    foreach (var piece in allPieces)
    //    {
    //        if (position == piece.Position && piece.Color == color)
    //        {
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    public static bool IsOccupiedBy(this Vector position, Color color, Piece?[,] board)
    {
        var possiblePiece = board[position.X, position.Y];
        if (possiblePiece is not null)
        {
            if (possiblePiece.Value.Color == color) return true;
        }
        return false;
    }

    public static IEnumerable<Vector> WithinBoard(this IEnumerable<Vector> positions)
    {
        return positions.Where(IsWithinBoard);
    }

    public static bool IsWithinBoard(this Vector position)
    {
        return position.X >= 0 && position.X < 8 && position.Y >= 0 && position.Y < 8;
    }

    public static IEnumerable<Move> WithinBoard(this IEnumerable<Move> moves)
    {
        return moves.Where(m => IsWithinBoard(m.PieceNewPosition));
    }
}
