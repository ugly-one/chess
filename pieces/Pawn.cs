using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Pawn : Piece
{
    public override Texture2D GetTexture()
    {
        return base.GetTexture(Player, "pawn");
    }

    public override Vector2[] GetMoves(Piece[] pieces)
    {
        var moves = new List<Vector2>();
        var direction = Player == Player.WHITE ? Vector2.Up : Vector2.Down;
        
        // one step forward if not blocked
        var forward = CurrentPosition + direction;
        if (!IsBlocked(forward, pieces))
            moves.Add(forward);
        
        // one down/left if there is an opponent's piece
        var takeLeft = CurrentPosition + Vector2.Left + direction;
        if (IsBlockedByOpponent(takeLeft, pieces))
        {
            moves.Add(takeLeft);
        }
        // one down/right if there is an opponent's piece
        var takeRight = CurrentPosition + Vector2.Right + direction;
        if (IsBlockedByOpponent(takeRight, pieces))
        {
            moves.Add(takeRight);
        }
        // two steps forward if not moved yet and not blocked
        if (Moved) return moves.ToArray();
        var forward2Steps = CurrentPosition + direction + direction;
        if (!IsBlocked(forward2Steps, pieces))
            moves.Add(forward2Steps);

        return moves.ToArray();
    }

    private bool IsBlockedByOpponent(Vector2 position, Piece[] pieces)
    {
        return pieces.Any(piece => piece.CurrentPosition == position && Player != piece.Player);
    }
    private bool IsBlocked(Vector2 position, Piece[] pieces)
    {
        return pieces.Any(piece => piece.CurrentPosition == position);
    }
}