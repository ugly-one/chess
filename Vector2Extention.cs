using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Vector2Extention
{
    /// <summary>
    /// this shouldn't be an extension method on player, but maybe we should have 2 methods GetWhiteTexture and GetBlackTexture
    /// that would ease the usage of it outside of PieceFactory class
    /// </summary>
    /// <param name="player"></param>
    /// <param name="piece"></param>
    /// <returns></returns>
    public static Texture2D GetTexture(this Player player, string piece)
    {
        var color = player == Player.WHITE ? "white" : "black";
        var image = Image.LoadFromFile("res://assets/" + color + "_" + piece + ".svg");
        return ImageTexture.CreateFromImage(image);
    }
    
    public static Player GetOppositePlayer(this Player player)
    {
        return player == Player.BLACK ? Player.WHITE : Player.BLACK;
    }
    
    public static IEnumerable<Vector2> GetDirection(
        this Vector2 currentPosition,
        Vector2 step, 
        IEnumerable<Piece> allPieces,
        Player player)
    {
        var newPos = currentPosition + step;
        var breakAfterAdding = false;
        while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(player, allPieces))
        {
            if (newPos.IsOccupiedBy(player.GetOppositePlayer(), allPieces))
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
    
    public static bool IsOccupiedBy(this Vector2 position, Player player, IEnumerable<Piece> allPieces)
    {
        foreach (var piece in allPieces)
        {
            if (position == piece.CurrentPosition && piece.Player == player)
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