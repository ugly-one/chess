using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Game
{
    public Board Board { get; private set; }
    public GameState State { get; private set; }
    public Color CurrentPlayer { get; private set; }
    public int MovesSinceLastPawnMoveOrPieceTake { get; private set; }
    
    public Game(ICollection<Piece> pieces)
    {
        Board = new Board(pieces);
        State = GameState.InProgress;
        CurrentPlayer = Color.WHITE;
        MovesSinceLastPawnMoveOrPieceTake = 0;
    }
    
    public Move? TryMove(Piece pieceToMove, Vector2 newPosition, PieceType? promotedPiece)
    {
        if (pieceToMove.Color != CurrentPlayer)
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
        CurrentPlayer = opponentsColor;
        
        if (move.PieceToMove.Type == PieceType.Pawn || move.PieceToCapture != null)
        {
            MovesSinceLastPawnMoveOrPieceTake = 0;
        }
        else
        {
            MovesSinceLastPawnMoveOrPieceTake += 1;
        }


        var possibleMovesForOpponent = Board.GetAllPossibleMovesForColor(opponentsColor);

        
        if (Board.IsKingUnderAttack(opponentsColor) && possibleMovesForOpponent.Count == 0)
        {
            State = pieceToMove.Color == Color.WHITE ? GameState.WhiteWin : GameState.BlackWin;
            return move;
        }
        
        if (MovesSinceLastPawnMoveOrPieceTake == 100)
        {
            State = GameState.Draw;
            return move;
        }
        
        if (possibleMovesForOpponent.Any())
        {
            return move;
        }
        
        // no possible moves, king not under attack
        State = GameState.Draw;
        return move;
    }

    public List<Move> GetPossibleMoves()
    {
        var possibleMoves = Board.GetAllPossibleMovesForColor(CurrentPlayer);
        return possibleMoves;
    }

    public Piece GetPiece(Vector2 position)
    {
        var piece = Board.GetPieces().First(p => p.Position == position);
        return piece;
    }
}