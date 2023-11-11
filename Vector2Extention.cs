using System.Collections.Generic;
using System.Linq;
using Chess;
using Godot;

public static class Vector2Extention
{
    /// <summary>
    /// this shouldn't be an extension method on player, but maybe we should have 2 methods GetWhiteTexture and GetBlackTexture
    /// that would ease the usage of it outside of PieceFactory class
    /// </summary>
    /// <param name="colorr"></param>
    /// <param name="piece"></param>
    /// <returns></returns>
    public static Texture2D GetTexture(this Color color, string piece)
    {
        var colorAsString = color == Color.WHITE ? "white" : "black";
        var image = Image.LoadFromFile("res://assets/" + colorAsString + "_" + piece + ".svg");
        return ImageTexture.CreateFromImage(image);
    }
    
    public static Color GetOppositeColor(this Color color)
    {
        return color == Color.BLACK ? Color.WHITE : Color.BLACK;
    }
    
    public static IEnumerable<Vector2> GetDirection(
        this Vector2 currentPosition,
        Vector2 step, 
        IEnumerable<Piece> allPieces,
        Color color)
    {
        var newPos = currentPosition + step;
        var breakAfterAdding = false;
        while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(color, allPieces))
        {
            if (newPos.IsOccupiedBy(color.GetOppositeColor(), allPieces))
            {
                breakAfterAdding = true;
            }
            yield return newPos;
            newPos += step;
            if (breakAfterAdding)
            {
                break;
            }
        }
    }
    
    public static bool IsOccupiedBy(this Vector2 position, Color color, IEnumerable<Piece> allPieces)
    {
        foreach (var piece in allPieces)
        {
            if (position == piece.CurrentPosition && piece.Color == color)
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
    
    public static IEnumerable<Vector2> WithinBoard(this IEnumerable<Vector2> positions)
    {
        return positions.Where(IsWithinBoard);
    }
}