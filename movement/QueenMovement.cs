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
        throw new System.NotImplementedException();
    }
}