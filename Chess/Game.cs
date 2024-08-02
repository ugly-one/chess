using System.Collections.Generic;
using System.Linq;

namespace Chess;

public class Game
{
    public Board Board { get; private set; }
    public GameState State { get; private set; }
    public int MovesSinceLastPawnMoveOrPieceTake { get; private set; }
    public Color CurrentPlayer => Board.CurrentPlayer;

    //public Game(Color startingColor, params Piece[] pieces)
    //{
    //    Board = new Board(pieces, startingColor);
    //    State = GameState.InProgress;
    //    MovesSinceLastPawnMoveOrPieceTake = 0;
    //}

    //public Game(ICollection<Piece> pieces)
    //{
    //    Board = new Board(pieces, Color.WHITE);
    //    State = GameState.InProgress;
    //    MovesSinceLastPawnMoveOrPieceTake = 0;
    //}

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

    public bool TryMove(Vector position, Vector newPosition, PieceType? promotedPiece)
    {
        var currentPlanyer = Board.CurrentPlayer;
        var pieceToMove = GetPiece(position);
        var (success, newBoard) = Board.TryMove(position, newPosition, promotedPiece);

        if (!success)
        {
            return false;
        }

        var previousBoard = Board;
        Board = newBoard;
        var opponentsColor = currentPlanyer.GetOpposite();

        var somethingWasCaptured = Board.GetPieces().Count() != previousBoard.GetPieces().Count();
        if (pieceToMove.Type == PieceType.Pawn || somethingWasCaptured)
        {
            MovesSinceLastPawnMoveOrPieceTake = 0;
        }
        else
        {
            MovesSinceLastPawnMoveOrPieceTake += 1;
        }

        if (HasInsufficientMatingMaterial(Board))
        {
            State = GameState.Draw;
            return true;
        }

        var possibleMovesForOpponent = Board.GetAllPossibleMoves();

        if (Board.IsKingUnderAttack(opponentsColor) && !possibleMovesForOpponent.Any())
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

    public bool HasInsufficientMatingMaterial(Board board)
    {
        var whitePieces = new List<Piece>();
        var blackPieces = new List<Piece>();

        foreach (var (piece, position) in board.GetPieces())
        {
            if (piece.Color == Color.WHITE)
                whitePieces.Add(piece);
            else
                blackPieces.Add(piece);
        }

        if (whitePieces.Count == 1 && blackPieces.Count == 1)
            // only 2 kings left
            return true;

        if (whitePieces.Count == 1 && HasOnlyKingAndBishopOrKnight(blackPieces))
            return true;

        if (blackPieces.Count == 1 && HasOnlyKingAndBishopOrKnight(whitePieces))
            return true;

        if (HasOnlyKingAndBishopOrKnight(whitePieces) && HasOnlyKingAndBishopOrKnight(blackPieces))
            return true;

        return false;
    }

    private static bool HasOnlyKingAndBishopOrKnight(List<Piece> pieces)
    {
        return pieces.Count == 2 &&
               pieces.Any(p => p.Type == PieceType.Bishop || p.Type == PieceType.Knight);
    }

    public Piece GetPiece(Vector position)
    {
        var piece = Board.GetPieces().First(x => x.Item2 == position).Item1;
        return piece;
    }
}
