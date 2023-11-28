using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Game
{
    public Board Board { get; private set; }
    public GameState State { get; private set; }
    
    private Color _currentPlayer;
    public Game(ICollection<Piece> pieces)
    {
        Board = new Board(pieces);
        State = GameState.InProgress;
        _currentPlayer = Color.WHITE;
    }
    
    public Move? TryMove(Piece pieceToMove, Vector2 newPosition, PieceType? promotedPiece)
    {
        if (pieceToMove.Color != _currentPlayer)
        {
            return null;
        }
        
        var possibleMoves = Board.GetPossibleMoves(pieceToMove);

        var move = possibleMoves.FirstOrDefault(m => m.PieceNewPosition == newPosition);
        
        if (move is null)
        {
            return null;
        }

        Board = Board.Move(move, promotedPiece);

        var opponentsColor = pieceToMove.Color.GetOppositeColor();
        _currentPlayer = opponentsColor;
        
        if (Board.GetAllPossibleMovesForColor(opponentsColor).Any())
        {
            return move;
        }
        
        if (Board.IsKingUnderAttack(opponentsColor))
        {
            State = pieceToMove.Color == Color.WHITE ? GameState.WhiteWin : GameState.BlackWin;
        }
        else
        {
            State = GameState.Draw;
        }
        return move;
    }
}