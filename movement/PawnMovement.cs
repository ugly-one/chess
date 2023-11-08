using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Bla.movement;

public partial class PawnMovement : Movement
{
    public override bool CanMove(Vector2 newPosition)
    {
        if (CurrentPosition.Y == newPosition.Y)
            return false;
        if (newPosition.X != CurrentPosition.X)
            return false;

        if (Player == Player.WHITE)
        {
            if (newPosition.Y + 1 == CurrentPosition.Y || (!Moved && newPosition.Y + 2 == CurrentPosition.Y))
                return true;
            return false;
        }

        if (newPosition.Y - 1 == CurrentPosition.Y || (!Moved && newPosition.Y - 2 == CurrentPosition.Y))
            return true;

        return false;
    }

    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "pawn");
    }

    public override Vector2[] GetMoves(Piece[] pieces, Vector2 currentPosition)
    {
        var moves = new List<Vector2>();
        if (Player == Player.WHITE)
        {
            // one step forward if not blocked
            var forward = currentPosition + Vector2.Up;
            if (!IsBlocked(forward, pieces))
                moves.Add(forward);
            
            // one down/left if there is an opponent's piece
            var takeLeft = currentPosition + Vector2.Left + Vector2.Up;
            if (IsBlockedByOpponent(takeLeft, pieces))
            {
                moves.Add(takeLeft);
            }
            // one down/right if there is an opponent's piece
            
            // two steps forward if not moved yet and not blocked
            if (Moved) return moves.ToArray();
            var forward2Steps = currentPosition + Vector2.Up + Vector2.Up;
            if (!IsBlocked(forward2Steps, pieces))
                moves.Add(forward2Steps);
        }

        return moves.ToArray();
    }

    private bool IsBlockedByOpponent(Vector2 position, Piece[] pieces)
    {
        return pieces.Any(piece => piece.Movement.CurrentPosition == position && Player != piece.Player);
    }
    private bool IsBlocked(Vector2 position, Piece[] pieces)
    {
        return pieces.Any(piece => piece.Movement.CurrentPosition == position);
    }
}