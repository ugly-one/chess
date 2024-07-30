using Chess;

namespace ChessAI;

public class GiveMeCheckAI  : IPlayer
{
    private Color color;

    public GiveMeCheckAI(Color color)
    {
        this.color = color;
    }

    public MoveWithPromotion GetMove(Board board)
    {
        var moveRequests = board.GetAllPossibleMovesForColor(color);
        foreach (var moveRequest in moveRequests)
        {
            var (success, newBoard) = board.TryMove(moveRequest.PieceToMove, moveRequest.PieceNewPosition, PieceType.Queen);
            if (!success)
                throw new Exception("The engine game me a move that is not valid :/");

            if (newBoard.IsKingUnderAttack(color.GetOpposite()))
                return new MoveWithPromotion(moveRequest.PieceToMove, moveRequest.PieceNewPosition, PieceType.Queen);
        }
        return new MoveWithPromotion(moveRequests[0].PieceToMove, moveRequests[0].PieceNewPosition, PieceType.Queen);
    }
}
