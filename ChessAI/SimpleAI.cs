using Chess;

namespace ChessAI;

public class SimpleAI : IPlayer
{
    public SimpleAI()
    {
    }

    public bool FoundMove { get; private set; }
    private (Piece, Vector, PieceType?) Move { get; set; }

    public (Piece, Vector, PieceType?) GetMove()
    {
        FoundMove = false;
        return Move;
    }

    public void FindMove(Game game)
    {
        FoundMove = false;
        Task.Run(() =>
        {
            Thread.Sleep(50);
            var possibleMoves = game.GetPossibleMoves();
            var randomIndex = new Random().Next(0, possibleMoves.Count());
            var randomMove = possibleMoves[randomIndex];
            if (randomMove.PieceToMove.Type == PieceType.Pawn && randomMove.PieceNewPosition.Y is 0 or 7)
            {
                Move = (randomMove.PieceToMove, randomMove.PieceNewPosition, PieceType.Queen);
            }
            else
            {
                Move = (randomMove.PieceToMove, randomMove.PieceNewPosition, null);
            }
            FoundMove = true;
        });
    }
}