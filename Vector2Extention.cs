using System.Collections.Generic;
using System.Linq;
using Bla.movement;
using Godot;

namespace Bla;

public static class Vector2Extention
{
    public static Player GetOppositePlayer(this Player player)
    {
        return player == Player.BLACK ? Player.WHITE : Player.BLACK;
    }
    
    public static IEnumerable<Vector2> GetDirection(
        this Vector2 currentPosition,
        Vector2 step, 
        IEnumerable<Movement> allPieces,
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
    
    public static bool IsOccupiedBy(this Vector2 position, Player player, IEnumerable<Movement> allPieces)
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