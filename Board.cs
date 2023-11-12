using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Board
{
    private Piece[] board;
    private Move lastMove;

    public Board(ICollection<Piece> board, Move lastMove = null)
    {
        this.board = board.ToArray();
        this.lastMove = lastMove;
    }
    
    /// <summary>
    /// Gets the king of given player
    /// </summary>
    /// <returns></returns>
    public Piece GetKing(Color color)
    {
        return board
            .First(k => k.Type == PieceType.King && k.Color == color);
    }

    public bool IsKingUnderAttack(Color color)
    {
        var king = GetKing(color);
        return IsPieceUnderAttack(king);
    }
    
    public bool IsPieceUnderAttack(Piece piece)
    {
        return IsFieldUnderAttack(piece.Position, piece.Color.GetOppositeColor());
    }
    
    /// <summary>
    /// Check if given field is under attack by given color's pieces
    /// </summary>
    /// <returns></returns>
    public bool IsFieldUnderAttack(Vector2 field, Color color)
    {
        var pieces = GetPieces(color);
        foreach (var piece in pieces)
        {
            var possibleMoves = GetMoves(piece);
            if (possibleMoves.Any(m => m.PieceNewPosition == field))
            {
                return true;
            }
        }
        return false;
    }

    public Piece[] GetPieces(Color color)
    {
        return board
            .Where(p => p.Color == color)
            .ToArray();
    }
    
    public Piece[] GetPieces()
    {
        return board
            .ToArray();
    }

    private Piece[] GetOppositeColorPieces(Color color)
    {
        return board
            .Where(p => p.Color != color)
            .ToArray();
    }
    
    /// <summary>
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    public Board Move(Move move, PieceType promotedPiece = PieceType.Queen)
    {
        var takenPiece = move.PieceToCapture;
        var newBoard = board.ToList();
        if (takenPiece != null)
        {
            newBoard = newBoard.Where(p => p != takenPiece).ToList();
        }
        var newPiece = move.PieceToMove.Move(move.PieceNewPosition);
        // convert a pawn into something better if necessary
        // this is horrible, I have the same logic in Main
        if (move.PieceToMove.Type == PieceType.Pawn && (move.PieceNewPosition.Y == 0 || move.PieceNewPosition.Y == 7))
        {
            newPiece = new Piece(promotedPiece, move.PieceToMove.Color, move.PieceNewPosition, moved: true);
        }
        newBoard = newBoard.Where(p => p != move.PieceToMove).Append(newPiece).ToList();

        var rockToMove = move.RockToMove;
        if (rockToMove != null)
        {
            var newRock = rockToMove.Move(move.RockNewPosition.Value);
            newBoard = newBoard.Where(p => p != move.RockToMove).Append(newRock).ToList();
        }
        newBoard.Remove(move.PieceToMove);

        return new Board(newBoard, move);
    }
    
    /// <summary>
    /// Get moves for the piece without taking into consideration all the rules
    /// TODO - maybe this should NOT take the board to be clear that it gives only moves that in theory a piece could do
    /// But then we should put the logic somewhere to detect that we can't jump over other pieces.
    /// However, it will be tricky because knights CAN jump over pieces and they do not have a range of movement in straight lines
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public Move[] GetMoves(Piece piece)
    {
        var moves = piece.Type switch
        {
            PieceType.King => GetKingMoves(piece),
            PieceType.Queen => GetQueenMoves(piece),
            PieceType.Pawn => GetPawnMoves(piece),
            PieceType.Bishop => GetBishopMoves(piece),
            PieceType.Rock => GetRockMoves(piece),
            PieceType.Knight => GetKnightMoves(piece)
        };
        return moves;
    }

    private Move[] GetQueenMoves(Piece piece)
    {
        var moves = new List<Move>();
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Right,   piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Left,    piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Left,  piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Right, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up,       piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down,   piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Left,   piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Right, piece.Color));
        return moves.ToArray();
    }

    private Move[] GetRockMoves(Piece piece)
    {
        var moves = new List<Move>();
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down,  piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Left,  piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up,    piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Right, piece.Color));
        return moves.ToArray();
    }

    private Move[] GetBishopMoves(Piece piece)
    {
        var moves = new List<Move>();
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Right,   piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Left,    piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Left,  piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Right, piece.Color));
        return moves.ToArray();
    }
    
    private Move[] GetPawnMoves(Piece piece)
    {
        var moves = new List<Move>();
        var direction = piece.Color == Color.WHITE ? Vector2.Up : Vector2.Down;
        
        // one step forward if not blocked
        var forward = piece.Position + direction;
        if (!IsBlocked(forward))
        {
            moves.Add(Chess.Move.RegularMove(piece, forward));
        }
        
        var opponentsPieces = GetOppositeColorPieces(piece.Color);
        // one down/left if there is an opponent's piece
        var takeLeft = piece.Position + Vector2.Left + direction;
        var opponentCapturedPiece = GetPieceInPosition(opponentsPieces, takeLeft);
        if (opponentCapturedPiece != null)
        {
            moves.Add(Chess.Move.Capture(piece, takeLeft, opponentCapturedPiece));
        }
        else
        {
            // TODO This is probably a bug - we shouldn't be using lastMove here
            // GetPawnMoves is executed when we simulate moves and that means lastMove is not updated correctly.
            // We don't update it when we simulate moves
            // check en-passant
            if (lastMove != null)
            {
                var isPawn = lastMove.PieceToMove.Type == PieceType.Pawn;
                var is2StepMove = (lastMove.PieceToMove.Position - lastMove.PieceNewPosition).Abs() == Vector2.Down * 2;
                var isThePawnNowNextToUs = (lastMove.PieceNewPosition - piece.Position) == Vector2.Left;
                if ( isPawn && // was it a pawn
                     is2StepMove && // was it a 2 step move
                     isThePawnNowNextToUs) // was it move next to us 
                {
                    moves.Add(Chess.Move.Capture(piece, takeLeft, GetPieceInPosition(opponentsPieces, lastMove.PieceNewPosition)));                
                }
            }
        }
        
        // one down/right if there is an opponent's piece
        var takeRight = piece.Position + Vector2.Right + direction;
        var opponentCapturedPiece2 = GetPieceInPosition(opponentsPieces, takeRight);
        if (opponentCapturedPiece2 != null)
        {
            moves.Add(Chess.Move.Capture(piece, takeRight, opponentCapturedPiece2));
        }
        else
        {
            // check en-passant
            if (lastMove != null)
            {
                var isPawn = lastMove.PieceToMove.Type == PieceType.Pawn;
                var is2StepMove = (lastMove.PieceToMove.Position - lastMove.PieceNewPosition).Abs() == Vector2.Down * 2;
                var isThePawnNowNextToUs = (lastMove.PieceNewPosition - piece.Position) == Vector2.Right;
                if ( isPawn && // was it a pawn
                     is2StepMove && // was it a 2 step move
                     isThePawnNowNextToUs) // was it move next to us 
                {
                    moves.Add(Chess.Move.Capture(piece, takeRight, GetPieceInPosition(opponentsPieces, lastMove.PieceNewPosition)));                
                }
            }
        }
        // two steps forward if not moved yet and not blocked
        if (piece.Moved) return moves.ToArray();
        var forward2Steps = piece.Position + direction + direction;
        if (!IsBlocked(forward2Steps))
        {
            moves.Add(Chess.Move.RegularMove(piece, forward2Steps));
        }

        return moves.ToArray();
    }

    private Piece GetPieceInPosition(Vector2 position)
    {
        return GetPieceInPosition(board, position);
    }
    
    private Piece GetPieceInPosition(Piece[] pieces, Vector2 position)
    {
        return pieces.FirstOrDefault(p => p.Position == position);
    }
    
    private bool IsBlocked(Vector2 position)
    {
        return board.Any(p => p.Position == position);
    }

    private Move[] GetKingMoves(Piece king)
    {
        var allPositions = new List<Vector2>()
        {
            king.Position + Vector2.Up,
            king.Position + Vector2.Down,
            king.Position + Vector2.Left,
            king.Position + Vector2.Right,
            king.Position + Vector2.Up + Vector2.Right,
            king.Position + Vector2.Up + Vector2.Left,
            king.Position + Vector2.Down + Vector2.Right,
            king.Position + Vector2.Down + Vector2.Left,
        };

        var allMoves = ConvertToMoves(king, allPositions).ToList();
        
        // short castle
        var shortCastleMove = TryGetCastleMove(king, Vector2.Right, 2);
        if (shortCastleMove != null)
        {
            allMoves.Add(shortCastleMove);
        }

        // long castle
        var longCastleMove = TryGetCastleMove(king, Vector2.Left, 3);
        if (longCastleMove != null)
        {
            allMoves.Add(longCastleMove);
        }
        
        return allMoves.ToArray();
    }

    public Move TryGetCastleMove(Piece king, Vector2 kingMoveDirection, int rockSteps)
    {
        if (king.Moved)
            return null;
        
        var rock = board
            .Where(p => p.Type == PieceType.Rock)
            .FirstOrDefault(p => p.Position == king.Position + kingMoveDirection * (rockSteps + 1));
        if (rock == null || rock.Moved) 
            return null;
        
        var allFieldsInBetweenClean = true;

        for (int i = 1; i <= 2; i++)
        {
            var fieldToCheck = king.Position + kingMoveDirection * i;
            if (GetPieceInPosition(fieldToCheck) != null)
            {
                allFieldsInBetweenClean = false;
                break;
            }
        }

        if (!allFieldsInBetweenClean) return null;
        
        var rockMoveDirection = kingMoveDirection.Orthogonal().Orthogonal();
        return Chess.Move.Castle(
            king,
            king.Position + kingMoveDirection * 2,
            rock,
            rock.Position + rockMoveDirection * rockSteps);

    }

    private Move[] GetKnightMoves(Piece piece)
    {
        var allPositions = new List<Vector2>()
        {
            piece.Position + Vector2.Up * 2 + Vector2.Right,
            piece.Position + Vector2.Right * 2 + Vector2.Up,
            piece.Position + Vector2.Right * 2 + Vector2.Down,
            piece.Position + Vector2.Down * 2 + Vector2.Right,
            piece.Position + Vector2.Down * 2 + Vector2.Left,
            piece.Position + Vector2.Left * 2 + Vector2.Down,
            piece.Position + Vector2.Up * 2 + Vector2.Left,
            piece.Position + Vector2.Left * 2 + Vector2.Up,
        };

        var allMoves = ConvertToMoves(piece, allPositions);
        return allMoves;
    }

    private Move[] ConvertToMoves(Piece piece, List<Vector2> allPositions)
    {
        return allPositions.Select(p =>
        {
            var pieceOnTheWay = GetPieceInPosition(p);
            if (pieceOnTheWay is null)
            {
                return Chess.Move.RegularMove(piece, p);
            }
            if (pieceOnTheWay.Color != piece.Color)
            {
                return Chess.Move.Capture(piece, p, pieceOnTheWay);
            }
            return null;
        }).Where(m => m != null).ToArray();
    }

    private IEnumerable<Move> GetMovesInDirection(
        Piece piece,
        Vector2 step, 
        Color color)
    {
        var newPos = piece.Position + step;
        var breakAfterAdding = false;
        while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(color, board))
        {
            // we should pass only opponents pieces to GetPieceInPosition
            var capturedPiece = GetPieceInPosition(newPos);
            if (capturedPiece != null)
            {
                breakAfterAdding = true;
                yield return Chess.Move.Capture(piece, newPos, capturedPiece);
                
            }
            else
            {
                yield return Chess.Move.RegularMove(piece, newPos);
            }
            newPos += step;
            if (breakAfterAdding)
            {
                break;
            }
        }
    }
}