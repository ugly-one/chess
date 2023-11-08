using System;
using System.Collections.Generic;
using Godot;

namespace Bla.movement;

public partial class RockMovement : Movement
{
    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "rock");
    }
    
    public override Vector2[] GetMoves(Piece[] pieces, Vector2 currentPosition)
    {
        var moves = new List<Vector2>();
        moves.AddRange(currentPosition.GetDirection(Vector2.Up, pieces, Player));
        moves.AddRange(currentPosition.GetDirection(Vector2.Down, pieces, Player));
        moves.AddRange(currentPosition.GetDirection(Vector2.Left, pieces, Player));
        moves.AddRange(currentPosition.GetDirection(Vector2.Right, pieces, Player));
        return moves.ToArray();
    }
}