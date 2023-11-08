using System;
using System.Collections.Generic;
using Godot;

namespace Bla.movement;

public partial class RockMovement : Movement
{
    public override bool CanMove(Vector2 newPosition)
    {
        if (CurrentPosition.Y == newPosition.Y)
            return true;
        if (newPosition.X == CurrentPosition.X)
            return true;
        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "rock");
    }

    public override Vector2[] GetMoves(Piece[] pieces, Vector2 currentPosition)
    {
        var moves = new List<Vector2>();

        //up 
        var distanceToTopEdge = Math.Abs(0 - currentPosition.Y); 
        for (var i = 1; i <= distanceToTopEdge ; i++)
        {
            moves.Add(currentPosition + Vector2.Up * i);
        }
        // down
        var distanceToBottomEdge = Math.Abs(7 - currentPosition.Y); 
        for (var i = 1; i <= distanceToBottomEdge ; i++)
        {
            moves.Add(currentPosition + Vector2.Down * i);
        }
        // left
        var distanceToLeftEdge = Math.Abs(0 - currentPosition.X); 
        for (var i = 1; i <= distanceToLeftEdge ; i++)
        {
            moves.Add(currentPosition + Vector2.Left * i);
        }
        // left
        var distanceToRightEdge = Math.Abs(7 - currentPosition.X); 
        for (var i = 1; i <= distanceToRightEdge ; i++)
        {
            moves.Add(currentPosition + Vector2.Right * i);
        }
        
        return moves.ToArray();
    }
}