using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess;

public class Board
{
    private readonly Piece?[,] board;
    private readonly Vector whiteKing;
    private readonly Vector blackKing;
    private readonly Move? lastMove;
    private readonly Color currentPlayer;

    public Color CurrentPlayer => currentPlayer;

    public Board(Piece?[,] pieces, Color currentColor, Move lastMove, Vector whiteKing, Vector blackKing)
    {
        this.currentPlayer = currentColor;
        this.lastMove = lastMove;
        this.board = pieces;
        this.whiteKing = whiteKing;
        this.blackKing = blackKing;
    }

    public Board(IEnumerable<(Piece, Vector)> board, Color currentPlayer = Color.WHITE, Move? lastMove = null)
    {
        this.currentPlayer = currentPlayer;
        this.board = new Piece?[8, 8];
        foreach (var (piece, position) in board)
        {
            this.board[position.X, position.Y] = piece;
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
        this.lastMove = lastMove;
    }

    public IEnumerable<(Piece, Vector)> GetPieces()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = board[x, y];
                if (piece != null)
                {
                    yield return (piece.Value, new Vector(x, y));
                }
            }
        }
    }

    public List<Move> GetAllPossibleMoves()
    {
        var fields = GetFieldsOccupiedBy(currentPlayer);
        var result = new List<Move>();
        foreach (var field in fields)
        {
            var piece = board[field.X, field.Y].Value;
            var possibleMoves = GetMoves(piece, field);
            foreach (var possibleMove in possibleMoves)
            {
                // let's try to make the move and see if the king is under attack, if yes, move is not allowed
                // it doesn't matter what we promote to
                var boardAfterMove = Move(possibleMove, PieceType.Queen);
                if (!boardAfterMove.IsKingUnderAttack(piece.Color))
                    result.Add(possibleMove);
            }
        }
        return result;
    }

    public (bool, Board) TryMove(Move move, PieceType? promotedPiece = null)
    {
        return TryMove(move.PieceOldPosition, move.PieceNewPosition, promotedPiece);
    }

    public (bool, Board) TryMove(Vector currentPosition, Vector newPosition, PieceType? promotedPiece = null)
    {
        var piece = board[currentPosition.X, currentPosition.Y].Value;
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
        var capturedPiece = board[move.PieceNewPosition.X, move.PieceNewPosition.Y];
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
                rockToMove = board[0, move.PieceNewPosition.Y];
            }
            else
            {
                rockNewPosition = new Vector(4, move.PieceNewPosition.Y);
                rockToMove = board[7, move.PieceNewPosition.Y];
            }
            newPieces[rockNewPosition.Value.X, rockNewPosition.Value.Y] = rockToMove.Value.Move();
        }

        Piece? en_passantCapturedPawn = null;
        if (move.Piece.Type == PieceType.Pawn && (move.PieceOldPosition - move.PieceNewPosition).Abs() == new Vector(1, 1) && board[move.PieceNewPosition.X, move.PieceNewPosition.Y] == null)
        {
            // en-passant 
            en_passantCapturedPawn = board[move.PieceNewPosition.X, move.PieceOldPosition.Y];
        }

        newPieces[move.PieceNewPosition.X, move.PieceNewPosition.Y] = movedPiece;

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var piece = board[x, y];
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
        if (IsAttackedHorizontalyOrVerticaly(position, color))
        {
            return true;
        }

        // check diagonal lines to see if there is a Queen or a Bishop
        if (IsAttackedDiagonally(position, color))
        {
            return true;
        }

        // check knights
        if (IsAttacked(knightDirections, position, PieceType.Knight, color))
        {
            return true;
        }

        // check king
        if (IsAttacked(kingDirections, position, PieceType.King, color))
        {
            return true;
        }

        // check pawns
        // this check is reverted because we want to know if the current position is under pawn's attack
        // maybe this method doesn't belong to Pawn file
        var attackPositions = color == Color.WHITE ? blackPawnAttackDirections : whitePawnAttackDirections;
        if (IsAttacked(attackPositions, position, PieceType.Pawn, color))
        {
            return true;
        }

        return false;
    }

    private IEnumerable<Vector> GetFieldsOccupiedBy(Color color)
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var piece = board[x, y];
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
    private List<Move> GetMoves(Piece piece, Vector position)
    {
        var moves = piece.Type switch
        {
            PieceType.King => GetKingMoves(piece, position),
            PieceType.Queen => GetQueenMoves(piece, position),
            PieceType.Pawn => GetPawnMoves(piece, position),
            PieceType.Bishop => GetBishopMoves(piece, position),
            PieceType.Rock => GetRockMoves(piece, position),
            PieceType.Knight => GetKnightMoves(piece, position),
            _ => throw new ArgumentOutOfRangeException()
        };
        return moves;
    }

    private static Vector[] whitePawnAttackDirections = new Vector[]
    {
        Vector.Up + Vector.Left,
        Vector.Up + Vector.Right
    };

    private static Vector[] blackPawnAttackDirections = new Vector[]
    {
        Vector.Down + Vector.Left,
        Vector.Down + Vector.Right
    };

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

    private static Vector[] queenDirections = new Vector[]
    {
        Vector.Up + Vector.Right,
        Vector.Up + Vector.Left,
        Vector.Down + Vector.Right,
        Vector.Down + Vector.Left,
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

    public List<Move> GetMovesInDirections(
        Piece piece,
        Vector currentPosition,
        Vector[] steps)
    {
        var result = new List<Move>();
        foreach (var step in steps)
        {
            var newPos = currentPosition + step;
            var breakAfterAdding = false;
            while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(piece.Color, board))
            {
                // we should pass only opponents pieces to GetPieceInPosition
                var capturedPiece = board[newPos.X, newPos.Y];
                if (capturedPiece != null)
                {
                    breakAfterAdding = true;
                    result.Add(new Move(piece, currentPosition, newPos));
                }
                else
                {
                    result.Add(new Move(piece, currentPosition, newPos));
                }
                newPos += step;
                if (breakAfterAdding)
                {
                    break;
                }
            }
        }
        return result;
    }

    public List<Move> GetBishopMoves(Piece piece, Vector position)
    {
        return GetMovesInDirections(piece, position, bishopDirections);
    }

    public List<Move> GetRockMoves(Piece piece, Vector position)
    {
        return GetMovesInDirections(piece, position, rockDirections);
    }

    public List<Move> GetQueenMoves(Piece piece, Vector position)
    {
        return GetMovesInDirections(piece, position, queenDirections);
    }

    public List<Move> GetKnightMoves(Piece piece, Vector position)
    {
        var allPositions = knightDirections.Select(d => d + position).WithinBoard();
        return Something.ConvertToMoves(piece, position, allPositions, board).ToList();
    }

    public List<Move> GetKingMoves(Piece king, Vector position)
    {
        var allPositions = kingDirections.Select(d => d + position).WithinBoard();

        var allMoves = Something.ConvertToMoves(king, position, allPositions, board).ToList();
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
                allMoves.Add(shortCastleMove);
            }
        }

        // long castle
        var longCastleMove = TryGetCastleMove(king, position, Vector.Right, 3);
        if (longCastleMove != null)
        {
            allMoves.Add(longCastleMove);
        }
        return allMoves;
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
        var rock = board[possibleRockPosition.X, possibleRockPosition.Y];

        if (rock == null || rock.Value.Moved)
            return null;

        var allFieldsInBetweenClean = true;

        for (int i = 1; i <= 2; i++)
        {
            var fieldToCheck = position + kingMoveDirection * i;
            if (board[fieldToCheck.X, fieldToCheck.Y] != null)
            {
                allFieldsInBetweenClean = false;
                break;
            }
        }

        if (!allFieldsInBetweenClean) return null;

        return new Move(king, position, position + kingMoveDirection * 2);
    }


    private bool IsAttacked(Vector[] positions, Vector position, PieceType pieceType, Color color)
    {
        foreach (var pos in positions)
        {
            var newPos = position + pos;
            if (newPos.IsWithinBoard())
            {
                var maybePiece = board[newPos.X, newPos.Y];
                if (maybePiece is Piece piece)
                {
                    if (piece.Type == pieceType && piece.Color == color)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool IsAttackedDiagonally(Vector position, Color color)
    {
        foreach (var direction in bishopDirections)
        {
            var maybePiece = position.GetTargetPieceInDirection(direction, board);
            if (maybePiece is Piece piece)
            {
                if (piece.Color == color && (piece.Type == PieceType.Queen || piece.Type == PieceType.Bishop))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsAttackedHorizontalyOrVerticaly(Vector position, Color color)
    {
        foreach (var direction in rockDirections)
        {
            var maybePiece = position.GetTargetPieceInDirection(direction, board);
            if (maybePiece is Piece piece)
            {
                if (piece.Color == color && (piece.Type == PieceType.Queen || piece.Type == PieceType.Rock))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<Move> GetPawnMoves(Piece piece, Vector position)
    {
        var direction = piece.Color == Color.WHITE ? Vector.Up : Vector.Down;

        var result = new List<Move>();
        // one step forward if not blocked
        var forward = position + direction;
        if (forward.IsWithinBoard() && !IsBlocked(forward))
        {
            result.Add(new Move(piece, position, forward));

            // two steps forward if not moved yet and not blocked
            if (!piece.Moved)
            {
                var forward2Steps = position + direction + direction;
                if (forward2Steps.IsWithinBoard() && !IsBlocked(forward2Steps))
                {
                    result.Add(new Move(piece, position, forward2Steps));
                }
            }
        }

        // one down/left if there is an opponent's piece
        var takeLeft = position + Vector.Left + direction;

        if (takeLeft.IsWithinBoard())
        {
            var possiblyCapturedPiece = board[takeLeft.X, takeLeft.Y];
            if (possiblyCapturedPiece != null && possiblyCapturedPiece.Value.Color != piece.Color)
            {
                result.Add(new Move(piece, position, takeLeft));
            }
            else
            {
                var move = TryGetEnPassant(piece, position, takeLeft);
                if (move != null)
                {
                    result.Add(move);
                }
            }
        }

        // one down/right if there is an opponent's piece
        var takeRight = position + Vector.Right + direction;
        if (takeRight.IsWithinBoard())
        {
            var possiblyCapturedPiece = board[takeRight.X, takeRight.Y];
            if (possiblyCapturedPiece != null && possiblyCapturedPiece.Value.Color != piece.Color)
            {
                result.Add(new Move(piece, position, takeRight));
            }
            else
            {
                var move = TryGetEnPassant(piece, position, takeRight);
                if (move != null)
                {
                    result.Add(move);
                }
            }
        }
        return result;
    }

    private Move? TryGetEnPassant(Piece piece, Vector currentPosition, Vector capturePosition)
    {
        if (lastMove == null) return null;

        var isPawn = lastMove.Piece.Type == PieceType.Pawn;
        var is2StepMove = (lastMove.PieceOldPosition - lastMove.PieceNewPosition).Abs() == Vector.Down * 2;
        var isThePawnNowNextToUs = (lastMove.PieceNewPosition - currentPosition) == new Vector((capturePosition - currentPosition).X, 0);
        if (isPawn && // was it a pawn
            is2StepMove && // was it a 2 step move
            isThePawnNowNextToUs) // was it move next to us
        {
            return new Move(piece, currentPosition, capturePosition);
        }

        return null;
    }

    private bool IsBlocked(Vector position)
    {
        if (board[position.X, position.Y] != null) return true;
        return false;
    }

    public override string ToString()
    {
        var stringRepresentation = new StringBuilder();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
                stringRepresentation.Append(board[x, y]?.ToString() ?? "\u00B7");

            stringRepresentation.Append('\n');
        }

        return stringRepresentation.ToString();
    }
}
