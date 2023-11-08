using System.Collections.Generic;
using Godot;

public partial class King : Piece
{
    public King(Player player, Vector2 position) : base(player, position)
    {
    }
    
    public override Texture2D GetTexture()
    {
        return Player.GetTexture("king");
    }

    public override Vector2[] GetMoves(Piece[] pieces)
    {
        // TODO king can step on its own pieces
        return new List<Vector2>()
        {
            CurrentPosition + Vector2.Up,
            CurrentPosition + Vector2.Down,
            CurrentPosition + Vector2.Left,
            CurrentPosition + Vector2.Right,
            CurrentPosition + Vector2.Up + Vector2.Right,
            CurrentPosition + Vector2.Up + Vector2.Left,
            CurrentPosition + Vector2.Down + Vector2.Right,
            CurrentPosition + Vector2.Down + Vector2.Left,
        }.ToArray();
    }

}