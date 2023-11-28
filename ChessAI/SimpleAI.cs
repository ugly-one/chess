using Chess;
using Godot;

namespace ChessAI;

public class SimpleAI
{
    public SimpleAI()
    {
    }

    /// <summary>
    /// Not good, now the AI cannot be used without GODOT library - due to usage of Vector2
    /// </summary>
    /// <param name="game"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public (Piece, Vector2) GetMove(Game game)
    {
        var possibleMoves = game.GetPossibleMoves();
        var randomIndex = new Random().Next(0, possibleMoves.Count());
        var randomMove = possibleMoves[randomIndex];
        return (randomMove.PieceToMove, randomMove.PieceNewPosition);
    }
}