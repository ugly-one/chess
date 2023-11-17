using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using Godot;

public static class Extentions
{
    public static string ToChessNotation(this Vector2 position)
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
        return letter+number;
    }
    
    public static Color GetOppositeColor(this Color color)
    {
        return color == Color.BLACK ? Color.WHITE : Color.BLACK;
    }
    
    public static bool IsOccupiedBy(this Vector2 position, Color color, IEnumerable<Piece> allPieces)
    {
        foreach (var piece in allPieces)
        {
            if (position == piece.Position && piece.Color == color)
            {
                return true;
            }
        }
        return false;
    }
    
    public static bool IsWithinBoard(this Vector2 position)
    {
        return position.X >= 0 && position.X < 8 && position.Y >= 0 && position.Y < 8;
    }
    
    public static IEnumerable<Move> WithinBoard(this IEnumerable<Move> moves)
    {
        return moves.Where(m => IsWithinBoard(m.PieceNewPosition));
    }
}