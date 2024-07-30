using System.Collections.Generic;
using System.Linq;

namespace Chess;

public class Game
{
    public Board Board { get; private set; }
    public GameState State { get; private set; }
    public int MovesSinceLastPawnMoveOrPieceTake { get; private set; }
    public Color CurrentPlayer => Board.CurrentPlayer;

    public Game(Color startingColor, params Piece[] pieces)
    {
        Board = new Board(pieces, startingColor);
        State = GameState.InProgress;
        MovesSinceLastPawnMoveOrPieceTake = 0;
    }

    public Game(ICollection<Piece> pieces)
    {
        Board = new Board(pieces, Color.WHITE);
        State = GameState.InProgress;
        MovesSinceLastPawnMoveOrPieceTake = 0;
    }

    public Game(Board board)
    {
        Board = board;
        State = GameState.InProgress;
        MovesSinceLastPawnMoveOrPieceTake = 0;
    }

    public Game()
    {
        Board = BoardFactory.Default();
        State = GameState.InProgress;
        MovesSinceLastPawnMoveOrPieceTake = 0;
    }
    
    public bool TryMove(Piece pieceToMove, Vector newPosition, PieceType? promotedPiece)
    {
        var (success, newBoard) = Board.TryMove(pieceToMove, newPosition, promotedPiece);

        if (!success)
        {
            return false;
        }

        var previousBoard = Board;
        Board = newBoard;
        var opponentsColor = pieceToMove.Color.GetOpposite();
        
        var somethingWasCaptured = Board.GetPieces().Count() != previousBoard.GetPieces().Count();
        if (pieceToMove.Type == PieceType.Pawn || somethingWasCaptured)
        {
            MovesSinceLastPawnMoveOrPieceTake = 0;
        }
        else
        {
            MovesSinceLastPawnMoveOrPieceTake += 1;
        }

        if (Board.HasInsufficientMatingMaterial())
        {
            State = GameState.Draw;
            return true;
        }

        var possibleMovesForOpponent = Board.GetAllPossibleMovesForColor(opponentsColor);
        
        if (Board.IsKingUnderAttack(opponentsColor) && possibleMovesForOpponent.Count == 0)
        {
            State = pieceToMove.Color == Color.WHITE ? GameState.WhiteWin : GameState.BlackWin;
            return true;
        }
        
        if (MovesSinceLastPawnMoveOrPieceTake == 100)
        {
            State = GameState.Draw;
            return true;
        }
        
        if (possibleMovesForOpponent.Any())
        {
            return true;
        }
        
        // no possible moves, king not under attack
        State = GameState.Draw;
        return true;
    }

    public Piece GetPiece(Vector position)
    {
        var piece = Board.GetPieces().First(p => p.Position == position);
        return piece;
    }
}
