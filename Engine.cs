using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Engine
{
    private Move lastMove;
    public Piece[] board;
    public Engine(List<Piece> board)
    {
        this.board = board.ToArray();
    }

    /// <summary>
    /// Checks possible moves for the given piece
    /// </summary>
    /// <param name="piece">piece for which possible moves will be calculated</param>
    /// <param name="board">entire board</param>
    /// <returns></returns>
    public Move[] GetPossibleMoves(Piece piece)
    {
        var possibleMoves = GetMoves(piece, board)
            .WithinBoard();

        var possibleMovesAfterFiltering = new List<Move>();
        foreach (var possibleMove in possibleMoves)
        {
            // let's try to make the move and see if the king is still under attack
            Piece[] boardAfterMove = Move(board, piece, possibleMove);
            // find the position of the king in the new setup,
            // We can't use the member variable because the king may moved after the move we simulate the move
            var king = GetKing(boardAfterMove, piece.Color);
            // if there is still check after this move - filter the move from possibleMoves
            var isUnderAttack = IsKingUnderAttack(boardAfterMove, king);
            if (!isUnderAttack)
            {
                possibleMovesAfterFiltering.Add(possibleMove);
            }
        }
        
        return possibleMovesAfterFiltering.ToArray();
    }

    public Move TryMove(Piece pieceToMove, Vector2 newPosition)
    {
        var possibleMoves = GetPossibleMoves(pieceToMove);

        var move = possibleMoves.FirstOrDefault(m => m.PieceNewPosition == newPosition);
        if (move is null)
        {
            return null;
        }
        
        var takenPiece = move.PieceToCapture; 
        if (takenPiece != null)
        {
            // we should remove the taken piece from the board because we need to evaluate if the king is checked/check-mated
            // and we shall do so with updated board
            board = board.Where(p => p != takenPiece).ToArray();
        }

        lastMove = move;
        move.PieceToMove.Move(move.PieceNewPosition);
        
        // did we manage to check opponent's king?
        var opponentsKing = GetKing(board, move.PieceToMove.Color.GetOppositeColor());
        var isOpponentsKingUnderFire = IsKingUnderAttack(board, opponentsKing);
        if (!isOpponentsKingUnderFire)
        {
            // check that the opponent have a move, if not - draw
            if (GetAllPossibleMovesForColor(board, opponentsKing.Color).Any())
            {
                return move;
            }
            GD.Print("DRAW!!");
            return move;
        }
        // opponent's king is under fine
        GD.Print("KING IS UNDER FIRE AFTER OUR MOVE");
        // did we manage to check-mate?
        if (GetAllPossibleMovesForColor(board, opponentsKing.Color).Any())
        {
            return move;
        }

        GD.Print("CHECK MATE!!");
        return move;
    }

    private List<Move> GetAllPossibleMovesForColor(Piece[] board, Color color)
    {
        var pieces = GetPieces(board, color);
        var allPossibleMoves = new List<Move>();
        foreach (var opponentsPiece in pieces)
        {
            // try to find possible moves
            var possibleMoves = GetPossibleMoves(opponentsPiece);
            allPossibleMoves.AddRange(possibleMoves);
        }

        return allPossibleMoves;
    }

    /// <summary>
    /// Gets the king if given player
    /// </summary>
    /// <returns></returns>
    private static Piece GetKing(Piece[] board, Color color)
    {
        return board
            .First(k => k.Type == PieceType.King && k.Color == color);
    }
    
    private bool IsKingUnderAttack(Piece[] board, Piece king)
    {
        var oppositePlayerPieces = GetOppositeColorPieces(board, king.Color);
        foreach (var oppositePlayerPiece in oppositePlayerPieces)
        {
            var possibleMoves = GetMoves(oppositePlayerPiece, board);
            var possibleCaptures = possibleMoves.Where(m => m.PieceToCapture != null);
            if (possibleCaptures.Any(m => m.PieceToCapture.Type == PieceType.King))
            {
                return true;
            }
        }
        return false;
    }

    private static Piece[] GetPieces(Piece[] board, Color color)
    {
        return board
            .Where(p => p.Color == color)
            .ToArray();
    }

    private static Piece[] GetOppositeColorPieces(Piece[] board, Color color)
    {
        return board
            .Where(p => p.Color != color)
            .ToArray();
    }
    
    private Piece[] Move(Piece[] board, Piece piece, Move move)
    {
        var boardCopy = board.ToList(); // shallow copy, do not modify pieces!
        boardCopy.Remove(piece);
        var takenPiece = move.PieceToCapture;
        if (takenPiece != null)
        {
            boardCopy.Remove(takenPiece);
        }
        var newPiece = piece.CloneWith(move.PieceNewPosition);
        boardCopy.Add(newPiece);
        return boardCopy.ToArray();
    }
    
    /// <summary>
    /// Get moves for the piece without taking into consideration all the rules
    /// TODO - maybe this should NOT take the board to be clear that it gives only moves that in theory a piece could do
    /// But then we should put the logic somewhere to detect that we can't jump over other pieces.
    /// However, it will be tricky because knights CAN jump over pieces and they do not have a range of movement in straight lines
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    private Move[] GetMoves(Piece piece, Piece[] board)
    {
        var moves = piece.Type switch
        {
            PieceType.King => GetKingMoves(piece, board),
            PieceType.Queen => GetQueenMoves(piece, board),
            PieceType.Pawn => GetPawnMoves(piece, board),
            PieceType.Bishop => GetBishopMoves(piece, board),
            PieceType.Rock => GetRockMoves(piece, board),
            PieceType.Knight => GetKnightMoves(piece, board)
        };
        return moves;
    }

    private Move[] GetQueenMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Right,   board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Left,    board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Left,  board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Right, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up, board,    piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down, board,  piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Left, board,  piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Right, board, piece.Color));
        return moves.ToArray();
    }

    private Move[] GetRockMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Left, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Right, board, piece.Color));
        return moves.ToArray();
    }

    private Move[] GetBishopMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Right, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Left, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Left, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Right, board, piece.Color));
        return moves.ToArray();
    }
    
    private Move[] GetPawnMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        var direction = piece.Color == Color.WHITE ? Vector2.Up : Vector2.Down;
        
        // one step forward if not blocked
        var forward = piece.CurrentPosition + direction;
        if (!IsBlocked(board, forward))
        {
            moves.Add(Chess.Move.RegularMove(piece, forward));
        }
        
        var opponentsPieces = GetOppositeColorPieces(board, piece.Color);
        // one down/left if there is an opponent's piece
        var takeLeft = piece.CurrentPosition + Vector2.Left + direction;
        var opponentCapturedPiece = GetPieceInPosition(opponentsPieces, takeLeft);
        if (opponentCapturedPiece != null)
        {
            moves.Add(Chess.Move.Capture(piece, takeLeft, opponentCapturedPiece));
        }
        else
        {
            // check en-passant
        }
        
        
        // one down/right if there is an opponent's piece
        var takeRight = piece.CurrentPosition + Vector2.Right + direction;
        var opponentCapturedPiece2 = GetPieceInPosition(opponentsPieces, takeRight);
        if (opponentCapturedPiece2 != null)
        {
            moves.Add(Chess.Move.Capture(piece, takeRight, opponentCapturedPiece2));
        }
        // two steps forward if not moved yet and not blocked
        if (piece.Moved) return moves.ToArray();
        var forward2Steps = piece.CurrentPosition + direction + direction;
        if (!IsBlocked(board, forward2Steps))
        {
            moves.Add(Chess.Move.RegularMove(piece, forward2Steps));
        }

        return moves.ToArray();
    }

    // private static bool EnPassantPossible(Move last2StepPawnMove, Vector2 takeRight)
    // {
    //     if (last2StepPawnMove is null)
    //     {
    //         return false;
    //     }
    //     if ((takeRight.Y > last2StepPawnMove.Start.Y && takeRight.Y < last2StepPawnMove.End.Y) ||
    //         (takeRight.Y < last2StepPawnMove.Start.Y && takeRight.Y > last2StepPawnMove.End.Y))
    //     {
    //         if (last2StepPawnMove.Start.X == takeRight.X)
    //         {
    //             return true;
    //         }
    //         return false;
    //     }
    //     return false;
    // }
    
    private Piece GetPieceInPosition(Piece[] pieces, Vector2 position)
    {
        return pieces.FirstOrDefault(p => p.CurrentPosition == position);
    }
    private bool IsBlocked(Piece[] pieces, Vector2 position)
    {
        return pieces.Any(p => p.CurrentPosition == position);
    }

    private Move[] GetKingMoves(Piece piece, Piece[] board)
    {
        var allPositions = new List<Vector2>()
        {
            piece.CurrentPosition + Vector2.Up,
            piece.CurrentPosition + Vector2.Down,
            piece.CurrentPosition + Vector2.Left,
            piece.CurrentPosition + Vector2.Right,
            piece.CurrentPosition + Vector2.Up + Vector2.Right,
            piece.CurrentPosition + Vector2.Up + Vector2.Left,
            piece.CurrentPosition + Vector2.Down + Vector2.Right,
            piece.CurrentPosition + Vector2.Down + Vector2.Left,
        };

        var allMoves = ConvertToMoves(piece, board, allPositions);
        return allMoves;
    }
    
    private Move[] GetKnightMoves(Piece piece, Piece[] board)
    {
        var allPositions = new List<Vector2>()
        {
            piece.CurrentPosition + Vector2.Up * 2 + Vector2.Right,
            piece.CurrentPosition + Vector2.Right * 2 + Vector2.Up,
            piece.CurrentPosition + Vector2.Right * 2 + Vector2.Down,
            piece.CurrentPosition + Vector2.Down * 2 + Vector2.Right,
            piece.CurrentPosition + Vector2.Down * 2 + Vector2.Left,
            piece.CurrentPosition + Vector2.Left * 2 + Vector2.Down,
            piece.CurrentPosition + Vector2.Up * 2 + Vector2.Left,
            piece.CurrentPosition + Vector2.Left * 2 + Vector2.Up,
        };

        var allMoves = ConvertToMoves(piece, board, allPositions);
        return allMoves;
    }

    private Move[] ConvertToMoves(Piece piece, Piece[] board, List<Vector2> allPositions)
    {
        return allPositions.Select(p =>
        {
            var pieceOnTheWay = GetPieceInPosition(board, p);
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
        Piece[] allPieces,
        Color color)
    {
        var newPos = piece.CurrentPosition + step;
        var breakAfterAdding = false;
        while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(color, allPieces))
        {
            // we should pass only opponents pieces to GetPieceInPosition
            var capturedPiece = GetPieceInPosition(allPieces, newPos);
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