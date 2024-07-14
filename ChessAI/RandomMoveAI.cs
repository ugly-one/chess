using Chess;

namespace ChessAI;

public class RandomMoveAI  : IPlayer
{
    private Color color;

    public RandomMoveAI(Color color)
    {
        this.color = color;
    }

    public Move GetMove(Board board)
    {
        var possibleMoves = board.GetAllPossibleMovesForColor(color);
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