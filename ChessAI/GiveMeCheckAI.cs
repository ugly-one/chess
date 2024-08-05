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
        var moveRequests = board.GetAllPossibleMoves().ToArray();
        foreach (var moveRequest in moveRequests)
        {
            var (success, newBoard) = board.TryMove(moveRequest.PieceOldPosition, moveRequest.PieceNewPosition, PieceType.Queen);
            if (!success)
                throw new Exception("The engine gave me a move that is not valid :/");
            
            if (newBoard.IsKingUnderAttack(currentKing: false))
                return new MoveWithPromotion(moveRequest.Piece, moveRequest.PieceOldPosition, moveRequest.PieceNewPosition, PieceType.Queen);
        }
        return new MoveWithPromotion(moveRequests[0].Piece, moveRequests[0].PieceOldPosition, moveRequests[0].PieceNewPosition, PieceType.Queen);
    }
}
