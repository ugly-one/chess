using System.Collections.Generic;
using Godot;

public partial class Rock : Piece
{
    public Rock(Player player, Vector2 position) : base(player, position)
    {
    }
    
    public override Texture2D GetTexture()
    {
        return Player.GetTexture("rock");
    }
    
    public override Vector2[] GetMoves(Piece[] pieces)
    {
        var moves = new List<Vector2>();
        moves.AddRange(CurrentPosition.GetDirection(Vector2.Up, pieces, Player));
        moves.AddRange(CurrentPosition.GetDirection(Vector2.Down, pieces, Player));
        moves.AddRange(CurrentPosition.GetDirection(Vector2.Left, pieces, Player));
        moves.AddRange(CurrentPosition.GetDirection(Vector2.Right, pieces, Player));
        return moves.ToArray();
    }

}