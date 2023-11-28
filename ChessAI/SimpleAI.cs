using Chess;
using Godot;

namespace ChessAI;

public class SimpleAI
{
    public SimpleAI()
    {
    }

    public bool FoundMove { get; private set; }
    private (Piece, Vector2) Move { get; set; }

    public (Piece, Vector2) GetMove()
    {
        return Move;
    }

    public void FindMove(Game game)
    {
        FoundMove = false;
        Task.Run(() =>
        {
            Thread.Sleep(1000);
            var possibleMoves = game.GetPossibleMoves();
            var randomIndex = new Random().Next(0, possibleMoves.Count());
            var randomMove = possibleMoves[randomIndex];
            Move = (randomMove.PieceToMove, randomMove.PieceNewPosition);
            FoundMove = true;
        });
    }
}