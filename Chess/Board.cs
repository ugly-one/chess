using System;
using System.Collections.Generic;
using System.Text;

namespace Chess;

public class Board
{
    private readonly Piece?[,] pieces;
    private readonly Vector whiteKing;
    private readonly Vector blackKing;
    private readonly Move? _lastMove;
    private readonly Color currentPlayer;

    public Piece?[,] Pieces => pieces;
    public Color CurrentPlayer => currentPlayer;

    public Board(Piece?[,] pieces, Color currentColor, Move lastMove, Vector whiteKing, Vector blackKing)
    {
        this.currentPlayer = currentColor;
        this._lastMove = lastMove;
        this.pieces = pieces;
        this.whiteKing = whiteKing;
        this.blackKing = blackKing;
    }

    public Board(IEnumerable<(Piece, Vector)> board, Color currentPlayer = Color.WHITE, Move? lastMove = null)
    {
        this.currentPlayer = currentPlayer;
        pieces = new Piece?[8, 8];
        foreach (var (piece, position) in board)
        {
            pieces[position.X, position.Y] = piece;
            if (piece.Color == Color.WHITE)
            {
                if (piece.Type == PieceType.King)
                {
                    whiteKing = position;
                }
            }
            else
            {
                if (piece.Type == PieceType.King)
                {
                    blackKing = position;
                }
            }

        }
        _lastMove = lastMove;
    }

    /// <summary>
    /// Checks possible moves for the given piece
    /// </summary>
    /// <param name="piece">piece for which possible moves will be calculated</param>
    /// <returns></returns>
    public IEnumerable<Move> GetPossibleMoves(Vector position, bool skipCache = false)
    {
        var piece = pieces[position.X, position.Y].Value;
        var possibleMoves = GetMoves(piece, position).WithinBoard();
        foreach (var possibleMove in possibleMoves)
        {
            // let's try to make the move and see if the king is under attack, if yes, move is not allowed
            // it doesn't matter what we promote to
            var boardAfterMove = Move(possibleMove, PieceType.Queen);
            if (boardAfterMove.IsKingUnderAttack(piece.Color)) continue;
            if (possibleMove.Piece.Type == PieceType.King)
            {
                var moveVector = (possibleMove.PieceNewPosition - possibleMove.PieceOldPosition);
                var isCastleMove = moveVector.Abs().X > 1;
                if (isCastleMove)
                {
                    // this check should be done as part of castle-move generation
                    var oneStepVector = moveVector.Clamp(new Vector(-1, -1), new Vector(1, 1));
                    if (IsFieldUnderAttack(possibleMove.PieceOldPosition + oneStepVector, possibleMove.Piece.Color.GetOpposite()))
                    {
                        // castling not allowed
                    }
                    else
                    {
                        yield return possibleMove;
                    }
                }
                else
                {
                    yield return possibleMove;
                }
            }
            else
            {
                yield return possibleMove;
            }
        }
    }

    public IEnumerable<(Piece, Vector)> GetPieces()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = pieces[x, y];
                if (piece != null)
                {
                    yield return (piece.Value, new Vector(x, y));
                }
            }
        }

    }

    public IEnumerable<Move> GetAllPossibleMoves()
    {
        var fields = GetFieldsOccupiedBy(currentPlayer);
        foreach (var field in fields)
        {
            // try to find possible moves
            var possibleMoves = GetPossibleMoves(field);
            foreach (var move in possibleMoves)
            {
                yield return move;
            }
        }

    }

    public (bool, Board) TryMove(Move move, PieceType? promotedPiece = null)
    {
        return TryMove(move.PieceOldPosition, move.PieceNewPosition, promotedPiece);
    }

    public (bool, Board) TryMove(Vector currentPosition, Vector newPosition, PieceType? promotedPiece = null)
    {
        var piece = pieces[currentPosition.X, currentPosition.Y].Value;
        var move = new Move(piece, currentPosition, newPosition);
        var newBoard = Move(move, promotedPiece);

        return (true, newBoard);
    }

    private Board Move(Move move, PieceType? promotedPiece)
    {
        var newPieces = new Piece?[8, 8];

        var movedPiece = move.Piece.Move();
        if (move.Piece.Type == PieceType.Pawn && (move.PieceNewPosition.Y == 0 || move.PieceNewPosition.Y == 7))
        {
            if (promotedPiece is null)
                movedPiece = movedPiece with { Type = PieceType.Queen };
            else
                movedPiece = movedPiece with { Type = promotedPiece.Value };

            newPieces[move.PieceNewPosition.X, move.PieceNewPosition.Y] = movedPiece;
        }

        Piece? pieceToRemove = null;
        var capturedPiece = pieces[move.PieceNewPosition.X, move.PieceNewPosition.Y];
        if (capturedPiece != null)
        {
            pieceToRemove = capturedPiece.Value;
        }
        Piece? rockToMove = null;
        if (move.Piece.Type == PieceType.King && !move.Piece.Moved && (move.PieceOldPosition - move.PieceNewPosition).Abs().X == 2)
        {
            Vector? rockNewPosition;
            if (move.PieceNewPosition.X == 1)
            {
                rockNewPosition = new Vector(2, move.PieceNewPosition.Y);
                rockToMove = pieces[0, move.PieceNewPosition.Y];
            }
            else
            {
                rockNewPosition = new Vector(4, move.PieceNewPosition.Y);
                rockToMove = pieces[7, move.PieceNewPosition.Y];
            }
            newPieces[rockNewPosition.Value.X, rockNewPosition.Value.Y] = rockToMove.Value.Move();
        }

        Piece? en_passantCapturedPawn = null;
        if (move.Piece.Type == PieceType.Pawn && (move.PieceOldPosition - move.PieceNewPosition).Abs() == new Vector(1, 1) && pieces[move.PieceNewPosition.X, move.PieceNewPosition.Y] == null)
        {
            // en-passant 
            en_passantCapturedPawn = pieces[move.PieceNewPosition.X, move.PieceOldPosition.Y];
        }

        newPieces[move.PieceNewPosition.X, move.PieceNewPosition.Y] = movedPiece;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var piece = pieces[x, y];
                if (piece is null) continue;
                var piece2 = piece.Value;
                if (pieceToRemove != null && (x == move.PieceNewPosition.X && y == move.PieceNewPosition.Y)) continue;
                if (move.PieceOldPosition.X == x && move.PieceOldPosition.Y == y) continue;
                if (move.PieceNewPosition.X == x && move.PieceNewPosition.Y == y) continue;
                if (rockToMove != null && piece2 == rockToMove) continue;
                if (en_passantCapturedPawn != null && piece2 == en_passantCapturedPawn) continue;
                newPieces[x, y] = piece2;
            }
        }

        var newWhiteKing = (movedPiece.Type == PieceType.King && currentPlayer == Color.WHITE) ? move.PieceNewPosition : whiteKing;
        var newBlackKing = (movedPiece.Type == PieceType.King && currentPlayer == Color.BLACK) ? move.PieceNewPosition : blackKing;

        return new Board(newPieces,
            currentPlayer.GetOpposite(),
            move,
            newWhiteKing,
            newBlackKing);
    }

    public bool IsKingUnderAttack(Color kingColor)
    {
        Vector king = kingColor == Color.WHITE ? whiteKing : blackKing;
        var oppositeColor = kingColor.GetOpposite();
        return IsFieldUnderAttack(king, oppositeColor);
    }

    /// Check if given position is under attack of a piece of a given color
    public bool IsFieldUnderAttack(Vector position, Color color)
    {
        // check horizontal/vertical lines to see if there is a Queen or a Rock
        var targets = Rock.GetTargets(position, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y].Value;
            if (targetPiece.Color == color &&
                (targetPiece.Type == PieceType.Queen ||
                targetPiece.Type == PieceType.Rock))
            {
                return true;
            }
        }
        // check diagonal lines to see if there is a Queen or a Bishop
        targets = Bishop.GetTargets(position, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y].Value;
            if (targetPiece.Color == color &&
                (targetPiece.Type == PieceType.Queen ||
                targetPiece.Type == PieceType.Bishop))
            {
                return true;
            }
        }
        // check knights
        targets = Knight.GetTargets(position, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y].Value;
            if (targetPiece.Color == color &&
                targetPiece.Type == PieceType.Knight)
            {
                return true;
            }
        }
        // check king
        targets = King.GetTargets(position, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y].Value;
            if (targetPiece.Color == color &&
                targetPiece.Type == PieceType.King)
            {
                return true;
            }
        }
        // check pawns
        targets = Pawn.GetTargets(position, color, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y].Value;
            if (targetPiece.Color == color &&
                targetPiece.Type == PieceType.Pawn)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerable<Vector> GetFieldsOccupiedBy(Color color)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = pieces[x, y];
                if (piece != null && piece.Value.Color == color)
                {
                    yield return new Vector(x, y);
                }
            }
        }
    }

    /// <summary>
    /// Get moves for the piece without taking into consideration all the rules
    /// But then we should put the logic somewhere to detect that we can't jump over other pieces.
    /// However, it will be tricky because knights CAN jump over pieces and they do not have a range of movement in straight lines
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    private IEnumerable<Move> GetMoves(Piece piece, Vector position)
    {
        var moves = piece.Type switch
        {
            PieceType.King => King.GetKingMoves(piece, position, pieces),
            PieceType.Queen => Queen.GetQueenMoves(piece, position, pieces),
            PieceType.Pawn => Pawn.GetPawnMoves(piece, position, pieces, _lastMove),
            PieceType.Bishop => Bishop.GetBishopMoves(piece, position, pieces),
            PieceType.Rock => Rock.GetRockMoves(piece, position, pieces),
            PieceType.Knight => Knight.GetKnightMoves(piece, position, pieces),
            _ => throw new ArgumentOutOfRangeException()
        };
        return moves;
    }

    public override string ToString()
    {
        var stringRepresentation = new StringBuilder();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
                stringRepresentation.Append(pieces[x, y]?.ToString() ?? "\u00B7");

            stringRepresentation.Append('\n');
        }

        return stringRepresentation.ToString();
    }
}
