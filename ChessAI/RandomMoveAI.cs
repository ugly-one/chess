using Chess;

namespace ChessAI;

public class RandomMoveAI  : IPlayer
{
    public Move GetMove(Game game)
    {
        var possibleMoves = game.GetPossibleMoves();
        var randomIndex = new Random().Next(0, possibleMoves.Count());
        var randomMove = possibleMoves[randomIndex];
        if (randomMove.PieceToMove.Type == PieceType.Pawn && randomMove.PieceNewPosition.Y is 0 or 7)
        {
            return randomMove with { PromotedType = PieceType.Queen};
        }
        else
        {
            return randomMove;
        }
    }
}
