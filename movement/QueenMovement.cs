using System.Collections.Generic;
using Godot;

namespace Bla.movement;

public partial class QueenMovement : Movement
{
    public override bool CanMove(Vector2 newPosition)
    {
        var moveVector = (newPosition - CurrentPosition).Abs();
        if (moveVector.X == moveVector.Y)
            return true;
        if (CurrentPosition.Y == newPosition.Y)
            return true;
        if (newPosition.X == CurrentPosition.X)
            return true;
        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "queen");
    }

    public override Vector2[] GetMoves(Piece[] pieces, Vector2 currentPosition)
    {
        var moves = new List<Vector2>();
        moves.AddRange(currentPosition.GetDirection(Vector2.Up + Vector2.Right));
        moves.AddRange(currentPosition.GetDirection(Vector2.Up + Vector2.Left));
        moves.AddRange(currentPosition.GetDirection(Vector2.Down + Vector2.Left));
        moves.AddRange(currentPosition.GetDirection(Vector2.Down + Vector2.Right));
        moves.AddRange(currentPosition.GetDirection(Vector2.Up));
        moves.AddRange(currentPosition.GetDirection(Vector2.Down));
        moves.AddRange(currentPosition.GetDirection(Vector2.Left));
        moves.AddRange(currentPosition.GetDirection(Vector2.Right));
        return moves.ToArray();
    }
}