using Chess;

namespace ChessAI;

public class GiveMeCheckAI  : IPlayer
{
    private Color color;

    public GiveMeCheckAI(Color color)
    {
        this.color = color;
    }

    public Move GetMove(Board board)
    {
        var possibleMoves = board.GetAllPossibleMovesForColor(color);
        foreach (var move in possibleMoves)
        {
            var newBoard = board.Move(move, PieceType.Queen);
            if (newBoard.IsKingUnderAttack(color.GetOppositeColor()))
                return move with {PromotedType = PieceType.Queen};
        }
        return possibleMoves[0] with {PromotedType = PieceType.Queen};
    }
}