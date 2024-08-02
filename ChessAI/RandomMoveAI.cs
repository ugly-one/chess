using Chess;

namespace ChessAI;

public class RandomMoveAI  : IPlayer
{
    private Color color;

    public RandomMoveAI(Color color)
    {
        this.color = color;
    }

    public MoveWithPromotion GetMove(Board board)
    {
        var possibleMoves = board.GetAllPossibleMoves().ToArray();
        var randomIndex = new Random().Next(0, possibleMoves.Count());
        var randomMove = possibleMoves[randomIndex];
        if (randomMove.Piece.Type == PieceType.Pawn && randomMove.PieceNewPosition.Y is 0 or 7)
        {
            return new MoveWithPromotion(randomMove.Piece, randomMove.PieceOldPosition, randomMove.PieceNewPosition, PieceType.Queen);
        }
        else
        {
            return new MoveWithPromotion(randomMove.Piece, randomMove.PieceOldPosition, randomMove.PieceNewPosition, PromotedPiece: null);
        }
    }
}
