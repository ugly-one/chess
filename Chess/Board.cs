using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess;

public class Board
{
    private readonly Piece?[,] board;
    private (Piece, Vector)? currentPlayerQueen;
    private List<(Piece, Vector)> currentPlayerRocks;
    private List<(Piece, Vector)> currentPlayerBishops;
    private List<(Piece, Vector)> currentPlayerKnights;
    private (Piece, Vector)? oppositePlayerQueen;
    private List<(Piece, Vector)> oppositePlayerRocks;
    private List<(Piece, Vector)> oppositePlayerBishops;
    private List<(Piece, Vector)> oppositePlayerKnights;
    private readonly Vector whiteKing;
    private readonly Vector blackKing;
    private readonly Move? lastMove;
    private readonly Color currentPlayer;
    private readonly Color oppositePlayer;

    public Color CurrentPlayer => currentPlayer;

    public Board(Piece?[,] pieces, Color currentColor, Move lastMove, Vector whiteKing, Vector blackKing)
    {
        this.currentPlayer = currentColor;
        this.oppositePlayer = currentColor.GetOpposite();
        this.lastMove = lastMove;
        this.board = pieces;
        this.whiteKing = whiteKing;
        this.blackKing = blackKing;
        currentPlayerRocks = new List<(Piece, Vector)>();
        currentPlayerBishops = new List<(Piece, Vector)>();
        currentPlayerKnights = new List<(Piece, Vector)>();
        this.oppositePlayerRocks = new List<(Piece, Vector)>();
        this.oppositePlayerBishops = new List<(Piece, Vector)>();
        this.oppositePlayerKnights = new List<(Piece, Vector)>();
        SetPieces();
    }

    public Board(IEnumerable<(Piece, Vector)> board, Color currentPlayer = Color.WHITE, Move? lastMove = null)
    {
        this.currentPlayer = currentPlayer;
        this.oppositePlayer = currentPlayer.GetOpposite();
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
        currentPlayerRocks = new List<(Piece, Vector)>();
        currentPlayerBishops = new List<(Piece, Vector)>();
        currentPlayerKnights = new List<(Piece, Vector)>();
        this.oppositePlayerRocks = new List<(Piece, Vector)>();
        this.oppositePlayerBishops = new List<(Piece, Vector)>();
        this.oppositePlayerKnights = new List<(Piece, Vector)>();
        SetPieces();
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

    public void SetPieces()
    {
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var maybePiece = board[x, y];
                if (maybePiece is Piece piece)
                {
                    if (piece.Type == PieceType.Queen)
                    {
                        if (piece.Color == currentPlayer)
                            currentPlayerQueen = (piece, new Vector(x, y));
                        else
                            oppositePlayerQueen = (piece, new Vector(x, y));
                    }
                    else if (piece.Type == PieceType.Rock)
                    {
                        if (piece.Color == currentPlayer)
                            currentPlayerRocks.Add((piece, new Vector(x, y)));
                        else
                            oppositePlayerRocks.Add((piece, new Vector(x, y)));
                    }
                    else if (piece.Type == PieceType.Bishop)
                    {
                        if (piece.Color == currentPlayer)
                            currentPlayerBishops.Add((piece, new Vector(x, y)));
                        else
                            oppositePlayerBishops.Add((piece, new Vector(x, y)));
                    }
                    else if (piece.Type == PieceType.Knight)
                    {
                        if (piece.Color == currentPlayer)
                            currentPlayerKnights.Add((piece, new Vector(x, y)));
                        else
                            oppositePlayerKnights.Add((piece, new Vector(x, y)));
                    }
                }
            }
        }
    }

    public List<Move> GetAllPossibleMoves()
    {
        var result = new List<Move>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                var maybePiece = board[x, y];
                if (maybePiece is Piece piece && maybePiece.Value.Color == currentPlayer)
                {
                    var field = new Vector(x, y);
                    switch (piece.Type)
                    {
                        case PieceType.King: GetKingMoves(piece, field, result); break;
                        case PieceType.Queen: GetQueenMoves(piece, field, result); break;
                        case PieceType.Pawn: GetPawnMoves(piece, field, result); break;
                        case PieceType.Bishop: GetBishopMoves(piece, field, result); break;
                        case PieceType.Rock: GetRockMoves(piece, field, result); break;
                        case PieceType.Knight: GetKnightMoves(piece, field, result); break;
                    };
                }
            }
        }
        return result;
    }

    private bool IsValid(Move move)
    {
        return !(Move(move, PieceType.Queen).IsKingUnderAttack(currentPlayer));
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

    ///CHecks if given field is under attack of current player's pieces. 
    public bool IsFieldUnderAttack2(Vector field, Color color)
    {
        var queen = color == currentPlayer ? currentPlayerQueen : oppositePlayerQueen;
        var rocks = color == currentPlayer ?  currentPlayerRocks : oppositePlayerRocks;
        var bishops = color == currentPlayer ? currentPlayerBishops : oppositePlayerBishops;
        var knights = color == currentPlayer ? currentPlayerKnights : oppositePlayerKnights;
        // queen
        if (queen is (Piece, Vector) a)
        {
            var queenPosition = a.Item2;
            var vector = (queenPosition - field);
            var notAttackedByQueen = true;
            var absVector = vector.Abs();
            if (absVector.Y == absVector.X || (queenPosition.X == field.X || queenPosition.Y == field.Y))
            {
                vector = vector.Clamp(new Vector(-1, -1), new Vector(1, 1));
                var inBetweenField = field + vector;
                notAttackedByQueen = false;
                while (inBetweenField != queenPosition)
                {
                    if (board[inBetweenField.X, inBetweenField.Y] != null)
                    {
                        notAttackedByQueen = true;
                        break;
                    }
                    inBetweenField += vector;
                }
            }
            if (!notAttackedByQueen) return true;

        }
        // rock
        foreach (var (_, rockPosition) in rocks)
        {
            var notAttackedByRock = true;
            if (rockPosition.X == field.X || rockPosition.Y == field.Y)
            {
                var vector = (rockPosition - field).Clamp(new Vector(-1, -1), new Vector(1, 1));
                var inBetweenField = field + vector;
                notAttackedByRock = false;
                while (inBetweenField != rockPosition)
                {
                    if (board[inBetweenField.X, inBetweenField.Y] != null)
                    {
                        notAttackedByRock = true;
                        break;
                    }
                    inBetweenField += vector;
                }
            }
            if (!notAttackedByRock) return true;
        }

        // bishop
        foreach (var (_, bishopPosition) in bishops)
        {
            var vector = (bishopPosition - field);
            var absVector = vector.Abs();
            var notAttackedByBishop = true;
            if (absVector.X == absVector.Y)
            {
                vector = vector.Clamp(new Vector(-1, -1), new Vector(1, 1));
                var inBetweenField = field + vector;
                notAttackedByBishop = false;
                while (inBetweenField != bishopPosition)
                {
                    if (board[inBetweenField.X, inBetweenField.Y] != null)
                    {
                        notAttackedByBishop = true;
                        break;
                    }
                    inBetweenField += vector;
                }
            }
            if (!notAttackedByBishop) return true;
        }

        // knights
        foreach(var (_, knightPosition) in knights)
        {
            var vector = (knightPosition - field).Abs();
            if ((vector.X == 2 && vector.Y == 1) || (vector.X == 1 && vector.Y == 2))
                return true;
        }
        return false;
    }

    /// Check if given position is under attack from a piece of a given color
    public bool IsFieldUnderAttack(Vector position, Color color)
    {
        // queen, rocks and bishops are done by the new method
        if (IsFieldUnderAttack2(position, color))
        {
            return true;
        }
        //// check horizontal/vertical lines to see if there is a Queen or a Rock
        //if (IsAttackedHorizontalyOrVerticaly(position, color))
        //{
        //    return true;
        //}

        //// check diagonal lines to see if there is a Queen or a Bishop
        //if (IsAttackedDiagonally(position, color))
        //{
        //    return true;
        //}

        // check knights
        //if (position.Y == 0)
        //{
        //    if (IsAttacked(knightTopRowAttackDirections, position, PieceType.Knight, color))
        //    {
        //        return true;
        //    }
        //}
        //else if (position.Y == 7)
        //{
        //    if (IsAttacked(knightBottomRowAttackDirections, position, PieceType.Knight, color))
        //    {
        //        return true;
        //    }
        //}
        //else if (position.X == 0)
        //{
        //    if (IsAttacked(knightLeftColumnAttackDirections, position, PieceType.Knight, color))
        //    {
        //        return true;
        //    }
        //}
        //else if (position.X == 7)
        //{
        //    if (IsAttacked(knightRightColumnAttackDirections, position, PieceType.Knight, color))
        //    {
        //        return true;
        //    }
        //}
        //else if (IsAttacked(knightInCenterAttackDirections, position, PieceType.Knight, color))
        //{
        //    return true;
        //}

        // check king
        if (position.Y == 0)
        {
            if (IsAttacked(kingTopDirections, position, PieceType.King, color))
            {
                return true;
            }
        }
        else if (position.Y == 7)
        {
            if (IsAttacked(kingBottomDirections, position, PieceType.King, color))
            {
                return true;
            }
        }
        else if (position.X == 0)
        {
            if (IsAttacked(kingLeftDirections, position, PieceType.King, color))
            {
                return true;
            }
        }
        else if (position.X == 7)
        {
            if (IsAttacked(kingRightDirections, position, PieceType.King, color))
            {
                return true;
            }
        }
        else if (IsAttacked(kingCenterDirections, position, PieceType.King, color))
        {
            return true;
        }

        // check pawns
        var attackPositions = color == Color.WHITE ? blackPawnAttackDirections : whitePawnAttackDirections;
        if (IsAttacked(attackPositions, position, PieceType.Pawn, color))
        {
            return true;
        }

        return false;
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

    private static Vector[] kingCenterDirections = new Vector[]
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

    private static Vector[] kingTopDirections = new Vector[]
    {
        Vector.Down,
        Vector.Left,
        Vector.Right,
        Vector.Down + Vector.Right,
        Vector.Down + Vector.Left,
    };

    private static Vector[] kingBottomDirections = new Vector[]
    {
        Vector.Up,
        Vector.Left,
        Vector.Right,
        Vector.Up + Vector.Right,
        Vector.Up + Vector.Left,
    };

    private static Vector[] kingLeftDirections = new Vector[]
    {
        Vector.Up,
        Vector.Down,
        Vector.Right,
        Vector.Up + Vector.Right,
        Vector.Down + Vector.Right,
    };

    private static Vector[] kingRightDirections = new Vector[]
    {
        Vector.Up,
        Vector.Down,
        Vector.Left,
        Vector.Up + Vector.Left,
        Vector.Down + Vector.Left,
    };


    private static Vector[] knightInCenterAttackDirections = new Vector[]
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

    private static Vector[] knightRightColumnAttackDirections = new Vector[]
    {
        Vector.Down * 2 + Vector.Left,
        Vector.Left * 2 + Vector.Down,
        Vector.Up * 2 + Vector.Left,
        Vector.Left * 2 + Vector.Up,
    };

    private static Vector[] knightLeftColumnAttackDirections = new Vector[]
    {
        Vector.Up * 2 + Vector.Right,
        Vector.Right * 2 + Vector.Up,
        Vector.Right * 2 + Vector.Down,
        Vector.Down * 2 + Vector.Right,
    };

    private static Vector[] knightTopRowAttackDirections = new Vector[]
    {
        Vector.Right * 2 + Vector.Down,
        Vector.Down * 2 + Vector.Right,
        Vector.Down * 2 + Vector.Left,
        Vector.Left * 2 + Vector.Down,
    };

    private static Vector[] knightBottomRowAttackDirections = new Vector[]
    {
        Vector.Up * 2 + Vector.Right,
        Vector.Right * 2 + Vector.Up,
        Vector.Up * 2 + Vector.Left,
        Vector.Left * 2 + Vector.Up,
    };

    public void GetMovesInDirections(
        Piece piece,
        Vector currentPosition,
        Vector[] steps,
        List<Move> result)
    {
        foreach (var step in steps)
        {
            var newPos = currentPosition + step;
            var breakAfterAdding = false;
            while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(piece.Color, board))
            {
                var capturedPiece = board[newPos.X, newPos.Y];
                if (capturedPiece != null)
                {
                    breakAfterAdding = true;
                    var move = new Move(piece, currentPosition, newPos);
                    if (IsValid(move))
                        result.Add(move);
                }
                else
                {
                    var move = new Move(piece, currentPosition, newPos);
                    if (IsValid(move))
                        result.Add(move);
                }
                newPos += step;
                if (breakAfterAdding)
                {
                    break;
                }
            }
        }
    }

    public void GetBishopMoves(Piece piece, Vector position, List<Move> result)
    {
        GetMovesInDirections(piece, position, bishopDirections, result);
    }

    public void GetRockMoves(Piece piece, Vector position, List<Move> result)
    {
        GetMovesInDirections(piece, position, rockDirections, result);
    }

    public void GetQueenMoves(Piece piece, Vector position, List<Move> result)
    {
        GetMovesInDirections(piece, position, queenDirections, result);
    }

    public void GetKnightMoves(Piece piece, Vector currentPosition, List<Move> result)
    {
        var allPositions = knightInCenterAttackDirections.Select(d => d + currentPosition).WithinBoard();

        foreach (var position in allPositions)
        {
            var pieceOnTheWay = board[position.X, position.Y];
            if (pieceOnTheWay is null)
            {
                var move = new Move(piece, currentPosition, position);
                if (IsValid(move)) result.Add(move);
            }
            else
            {
                if (pieceOnTheWay.Value.Color != piece.Color)
                {
                    var move = new Move(piece, currentPosition, position);
                    if (IsValid(move)) result.Add(move);
                }
            }
        }
    }

    public void GetKingMoves(Piece piece, Vector currentPosition, List<Move> result)
    {
        var allPositions = kingCenterDirections.Select(d => d + currentPosition).WithinBoard();

        foreach (var position in allPositions)
        {
            var pieceOnTheWay = board[position.X, position.Y];
            if (pieceOnTheWay is null)
            {
                var move = new Move(piece, currentPosition, position);
                if (IsValid(move)) result.Add(move);
            }
            else
            {
                if (pieceOnTheWay.Value.Color != piece.Color)
                {
                    var move = new Move(piece, currentPosition, position);
                    if (IsValid(move)) result.Add(move);
                }
            }
        }

        // short castle
        if (piece.Moved)
            return;

        if (IsFieldUnderAttack(currentPosition, oppositePlayer))
        {
            return;
        }
        var maybeRock = board[0, currentPosition.Y];
        if (maybeRock is Piece shortRock && !shortRock.Moved)
        {
            var allFieldsInBetweenClean = true;
            var fieldsToCheck = new Vector[]
            {
                new Vector(1, currentPosition.Y),
                new Vector(2, currentPosition.Y)
            };
            foreach (var fieldToCheck in fieldsToCheck)
            {
                if (board[fieldToCheck.X, fieldToCheck.Y] != null)
                {
                    allFieldsInBetweenClean = false;
                    break;
                }
                if (IsFieldUnderAttack(fieldToCheck, oppositePlayer))
                {
                    allFieldsInBetweenClean = false;
                    break;
                }
            }

            if (allFieldsInBetweenClean)
            {
                var move = new Move(piece, currentPosition, new Vector(1, currentPosition.Y));
                result.Add(move);
            }
        }

        // long castle
        maybeRock = board[7, currentPosition.Y];
        if (maybeRock is Piece longRock && !longRock.Moved)
        {
            var allFieldsInBetweenClean = true;
            var fieldsToCheck = new Vector[]
            {
                new Vector(6, currentPosition.Y),
                new Vector(5, currentPosition.Y),
                new Vector(4, currentPosition.Y)
            };
            foreach (var fieldToCheck in fieldsToCheck)
            {
                if (board[fieldToCheck.X, fieldToCheck.Y] != null)
                {
                    allFieldsInBetweenClean = false;
                    break;
                }
                if (IsFieldUnderAttack(fieldToCheck, oppositePlayer))
                {
                    allFieldsInBetweenClean = false;
                    break;
                }
            }

            if (allFieldsInBetweenClean)
            {
                var move = new Move(piece, currentPosition, new Vector(5, currentPosition.Y));
                result.Add(move);
            }
        }
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
            var maybePiece = GetTargetPieceInDirection(position, direction);
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
            var maybePiece = GetTargetPieceInDirection(position, direction);
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

    public void GetPawnMoves(Piece piece, Vector position, List<Move> result)
    {
        var direction = currentPlayer == Color.WHITE ? Vector.Up : Vector.Down;

        var forward = position + direction;
        if (!forward.IsWithinBoard())
            return;

        var takeLeft = forward + Vector.Left;
        var takeRight = forward + Vector.Right;

        if (!IsBlocked(forward))
        {
            var move = new Move(piece, position, forward);
            if (IsValid(move)) result.Add(move);

            // two steps forward if not moved yet and not blocked
            if (!piece.Moved)
            {
                var forward2Steps = forward + direction;
                if (forward2Steps.IsWithinBoard() && !IsBlocked(forward2Steps))
                {
                    move = new Move(piece, position, forward2Steps);
                    if (IsValid(move)) result.Add(move);
                }
            }
        }

        // one down/left if there is an opponent's piece
        if (takeLeft.X >= 0)
        {
            var possiblyCapturedPiece = board[takeLeft.X, takeLeft.Y];
            if (possiblyCapturedPiece != null && possiblyCapturedPiece.Value.Color != currentPlayer)
            {
                var move = new Move(piece, position, takeLeft);
                if (IsValid(move)) result.Add(move);
            }
            else
            {
                var move = TryGetEnPassant(piece, position, takeLeft);
                if (move != null)
                {
                    if (IsValid(move)) result.Add(move);
                }
            }
        }

        // one down/right if there is an opponent's piece
        if (takeRight.X <= 7)
        {
            var possiblyCapturedPiece = board[takeRight.X, takeRight.Y];
            if (possiblyCapturedPiece != null && possiblyCapturedPiece.Value.Color != currentPlayer)
            {
                var move = new Move(piece, position, takeRight);
                if (IsValid(move)) result.Add(move);
            }
            else
            {
                var move = TryGetEnPassant(piece, position, takeRight);
                if (move != null)
                {
                    if (IsValid(move)) result.Add(move);
                }
            }
        }
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

    public Piece? GetTargetPieceInDirection(
            Vector position,
            Vector direction)
    {
        var newPos = position + direction;
        while (newPos.IsWithinBoard())
        {
            var pieceAtNewPosition = board[newPos.X, newPos.Y];
            if (pieceAtNewPosition != null)
            {
                return pieceAtNewPosition;
            }
            newPos += direction;
        }
        return null;
    }

    public override string ToString()
    {
        var stringRepresentation = new StringBuilder();
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
                stringRepresentation.Append(board[x, y]?.ToString() ?? "\u00B7");

            stringRepresentation.Append('\n');
        }

        return stringRepresentation.ToString();
    }
}
