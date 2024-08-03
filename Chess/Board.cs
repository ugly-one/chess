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
            if (boardAfterMove.IsKingUnderAttack(piece.Color))
                continue;
            yield return possibleMove;
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
        var targets = GetTargets2(rockDirections, position);
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
        targets = GetTargets2(bishopDirections, position);
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
        targets = GetTargets(knightDirections, position);
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
        targets = GetTargets(kingDirections, position);
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
            PieceType.King => GetKingMoves(piece, position),
            PieceType.Queen => Queen.GetQueenMoves(piece, position, pieces),
            PieceType.Pawn => Pawn.GetPawnMoves(piece, position, pieces, _lastMove),
            PieceType.Bishop => GetBishopMoves(piece, position),
            PieceType.Rock => GetRockMoves(piece, position, pieces),
            PieceType.Knight => GetKnightMoves(piece, position),
            _ => throw new ArgumentOutOfRangeException()
        };
        return moves;
    }

    private static Vector[] bishopDirections = new Vector[]
    {
        Vector.Up + Vector.Right,
        Vector.Up + Vector.Left,
        Vector.Down + Vector.Right,
        Vector.Down + Vector.Left,
    };

    private static Vector[] rockDirections = new Vector[]
    {
        Vector.Up,
        Vector.Down,
        Vector.Left,
        Vector.Right
    };

    private static Vector[] kingDirections = new Vector[]
    {
        Vector.Up,
        Vector.Down,
        Vector.Left,
        Vector.Right,
        Vector.Up + Vector.Right,
        Vector.Up + Vector.Left,
        Vector.Down + Vector.Right,
        Vector.Down + Vector.Left,
    };

    private static Vector[] knightDirections = new Vector[]
    {
        Vector.Up * 2 + Vector.Right,
        Vector.Right * 2 + Vector.Up,
        Vector.Right * 2 + Vector.Down,
        Vector.Down * 2 + Vector.Right,
        Vector.Down * 2 + Vector.Left,
        Vector.Left * 2 + Vector.Down,
        Vector.Up * 2 + Vector.Left,
        Vector.Left * 2 + Vector.Up,
    };

    public IEnumerable<Move> GetBishopMoves(Piece piece, Vector position)
    {
        foreach (var direction in bishopDirections)
        {
            foreach (var move in pieces.GetMovesInDirection(piece, position, direction, piece.Color))
            {
                yield return move;
            }
        }
    }

    public IEnumerable<Move> GetRockMoves(Piece piece, Vector position, Piece?[,] board)
    {
        foreach (var direction in rockDirections)
        {
            foreach (var move in board.GetMovesInDirection(piece, position, direction, piece.Color))
            {
                yield return move;
            }
        }
    }

    public IEnumerable<Move> GetKnightMoves(Piece piece, Vector position)
    {
        var allPositions = new Vector[]
        {
            position + Vector.Up * 2 + Vector.Right,
            position + Vector.Right * 2 + Vector.Up,
            position + Vector.Right * 2 + Vector.Down,
            position + Vector.Down * 2 + Vector.Right,
            position + Vector.Down * 2 + Vector.Left,
            position + Vector.Left * 2 + Vector.Down,
            position + Vector.Up * 2 + Vector.Left,
            position + Vector.Left * 2 + Vector.Up,
        }.WithinBoard();
        return Something.ConvertToMoves(piece, position, allPositions, pieces);
    }

    public IEnumerable<Move> GetKingMoves(Piece king, Vector position)
    {
        var allPositions = new Vector[]
        {
            position + Vector.Up,
            position + Vector.Down,
            position + Vector.Left,
            position + Vector.Right,
            position + Vector.Up + Vector.Right,
            position + Vector.Up + Vector.Left,
            position + Vector.Down + Vector.Right,
            position + Vector.Down + Vector.Left,
        }.WithinBoard();

        var allMoves = Something.ConvertToMoves(king, position, allPositions, pieces);
        foreach (var move in allMoves)
        {
            yield return move;
        }
        // short castle
        var shortCastleMove = TryGetCastleMove(king, position, Vector.Left, 2);
        if (shortCastleMove != null)
        {
            // this check should be done as part of castle-move generation
            var moveVector = (shortCastleMove.PieceNewPosition - shortCastleMove.PieceOldPosition);
            var oneStepVector = moveVector.Clamp(new Vector(-1, -1), new Vector(1, 1));
            if (IsFieldUnderAttack(shortCastleMove.PieceOldPosition + oneStepVector, shortCastleMove.Piece.Color.GetOpposite()))
            {
                // castling not allowed
            }
            else
            {
                yield return shortCastleMove;
            }
        }

        // long castle
        var longCastleMove = TryGetCastleMove(king, position, Vector.Right, 3);
        if (longCastleMove != null)
        {
            yield return longCastleMove;
        }
    }

    private Move? TryGetCastleMove(Piece king, Vector position, Vector kingMoveDirection, int rockSteps)
    {
        if (king.Moved)
            return null;

        var possibleRockPosition = position + kingMoveDirection * (rockSteps + 1);
        if (!possibleRockPosition.IsWithinBoard())
        {
            // TODO I'm wasting time here. I shouldn't even consider such position. 
            // finding both rocks for king's position should be as simple as doing 2 lookups in the board
            // the position of rocks never change. And if it changed (and we can't find the rock where it should be) - no castling
            // It may change for Chess960, but for now we could have 2 hardcoded positions to check
            return null;
        }

        // TODO check that the piece we got here is actually a rock
        var rock = pieces[possibleRockPosition.X, possibleRockPosition.Y];

        if (rock == null || rock.Value.Moved)
            return null;

        var allFieldsInBetweenClean = true;

        for (int i = 1; i <= 2; i++)
        {
            var fieldToCheck = position + kingMoveDirection * i;
            if (pieces[fieldToCheck.X, fieldToCheck.Y] != null)
            {
                allFieldsInBetweenClean = false;
                break;
            }
        }

        if (!allFieldsInBetweenClean) return null;

        return new Move(king, position, position + kingMoveDirection * 2);
    }

    public IEnumerable<Vector> GetTargets(IEnumerable<Vector> positions, Vector position)
    {
        foreach (var pos in positions)
        {
            var newPos = position + pos;
            var target = newPos.GetTargetInPosition(pieces);
            if (target != null)
                yield return target.Value;
        }
    }

    public IEnumerable<Vector> GetTargets2(IEnumerable<Vector> directions, Vector position)
    {
        foreach (var direction in directions)
        {
            var target = position.GetTargetInDirection(direction, pieces);
            if (target != null)
                yield return target.Value;
        }
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
