using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Engine
{
    /// <summary>
    /// Checks possible moves for the given piece
    /// </summary>
    /// <param name="piece">piece for which possible moves will be calculated</param>
    /// <param name="board">entire board</param>
    /// <returns></returns>
    public Vector2[] GetPossibleMoves(Piece piece, Piece[] board)
    {
        var possibleMoves = GetMoves(piece, board)
            .WithinBoard();

        var possibleMovesAfterFiltering = new List<Vector2>();
        foreach (var possibleMove in possibleMoves)
        {
            // let's try to make the move and see if the king is still under attack
            Piece[] boardAfterMove = Move(board, piece, possibleMove);
            // find the position of the king in the new setup,
            // We can't use the member variable because the king may moved after the move we simulate the move
            var king = boardAfterMove.First(k => k.Type == PieceType.King && k.Player == piece.Player);
            // if there is still check after this move - filter the move from possibleMoves
            var isUnderAttack = IsKingUnderAttack(boardAfterMove, king);
            if (!isUnderAttack)
            {
                possibleMovesAfterFiltering.Add(possibleMove);
            }
        }
        
        return possibleMovesAfterFiltering.ToArray();
    }
    
    private bool IsKingUnderAttack(Piece[] board, Piece king)
    {
        var oppositePlayerPieces = board
            .Where(p => p.Player != king.Player);
        foreach (var oppositePlayerPiece in oppositePlayerPieces)
        {
            var possibleMoves = GetMoves(oppositePlayerPiece, board);
            if (possibleMoves.Contains(king.CurrentPosition))
            {
                GD.Print("KING UNDER FIRE");
                return true;
            }
        }
        return false;
    }
    
    public bool TryMove(Piece pieceToMove, Piece[] board, Vector2 newPosition)
    {
        var possibleMoves = GetPossibleMoves(pieceToMove, board);
		
        if (!possibleMoves.Contains(newPosition))
        {
            return false;
        }
        
        var takenPiece = board.FirstOrDefault(p => p.CurrentPosition == newPosition);
        if (takenPiece != null)
        {
            takenPiece.Kill();
        }
        pieceToMove.Move(newPosition);
        return true;
    }

    private Piece[] Move(Piece[] board, Piece piece, Vector2 move)
    {
        var boardCopy = board.ToList(); // shallow copy, do not modify pieces!
        boardCopy.Remove(piece);
        // I'm afraid I will have to duplicate the logic of making a move here.
        // move can be done in 3 different ways:
        // a) simple move where only the moved piece is affected
        // b) capture - moved piece is affected and the captured piece has to be removed
        // c) castling - rock and king are affected
        // now here we have a support for a and b. 
        // but the same logic will have to reside somewhere else where we actually make the move once the engine detects that the move is valid
        // currently it's the main class.
        // simulate taking a piece
        var takenPiece = boardCopy.FirstOrDefault(p => p.CurrentPosition == move);
        if (takenPiece != null)
        {
            boardCopy.Remove(takenPiece);
        }
        var newPiece = piece.CloneWith(move);
        boardCopy.Add(newPiece);
        return boardCopy.ToArray();
    }

    private Vector2[] GetMoves(Piece piece, Piece[] board)
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

    private Vector2[] GetQueenMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Vector2>();
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Up + Vector2.Right,   board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Up + Vector2.Left,    board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Down + Vector2.Left,  board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Down + Vector2.Right, board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Up, board,    piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Down, board,  piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Left, board,  piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Right, board, piece.Player));
        return moves.ToArray();
    }

    private Vector2[] GetKnightMoves(Piece piece, Piece[] board)
    {
        // TODO king can step on its own pieces
        var moves = new List<Vector2>();
        moves.Add(piece.CurrentPosition + Vector2.Up * 2 + Vector2.Right);
        moves.Add(piece.CurrentPosition + Vector2.Right * 2 + Vector2.Up);
        moves.Add(piece.CurrentPosition + Vector2.Right * 2 + Vector2.Down);
        moves.Add(piece.CurrentPosition + Vector2.Down * 2 + Vector2.Right);
        moves.Add(piece.CurrentPosition + Vector2.Down * 2 + Vector2.Left);
        moves.Add(piece.CurrentPosition + Vector2.Left * 2 + Vector2.Down);
        moves.Add(piece.CurrentPosition + Vector2.Up * 2 + Vector2.Left);
        moves.Add(piece.CurrentPosition + Vector2.Left * 2 + Vector2.Up);
        return moves.ToArray();
    }

    private Vector2[] GetRockMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Vector2>();
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Down, board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Left, board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Up, board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Right, board, piece.Player));
        return moves.ToArray();
    }

    private Vector2[] GetBishopMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Vector2>();
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Up + Vector2.Right, board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Up + Vector2.Left, board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Down + Vector2.Left, board, piece.Player));
        moves.AddRange(piece.CurrentPosition.GetDirection(Vector2.Down + Vector2.Right, board, piece.Player));
        return moves.ToArray();
    }

    private Vector2[] GetPawnMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Vector2>();
        var direction = piece.Player == Player.WHITE ? Vector2.Up : Vector2.Down;
        
        // one step forward if not blocked
        var forward = piece.CurrentPosition + direction;
        if (!IsBlocked(forward, board))
            moves.Add(forward);
        
        // one down/left if there is an opponent's piece
        var takeLeft = piece.CurrentPosition + Vector2.Left + direction;
        if (IsBlockedByOpponent(piece, takeLeft, board))
        {
            moves.Add(takeLeft);
        }
        // one down/right if there is an opponent's piece
        var takeRight = piece.CurrentPosition + Vector2.Right + direction;
        if (IsBlockedByOpponent(piece, takeRight, board))
        {
            moves.Add(takeRight);
        }
        // two steps forward if not moved yet and not blocked
        if (piece.Moved) return moves.ToArray();
        var forward2Steps = piece.CurrentPosition + direction + direction;
        if (!IsBlocked(forward2Steps, board))
            moves.Add(forward2Steps);

        return moves.ToArray();
    }
    
    private bool IsBlockedByOpponent(Piece piece, Vector2 position, Piece[] pieces)
    {
        return pieces.Any(p => p.CurrentPosition == position && piece.Player != p.Player);
    }
    private bool IsBlocked(Vector2 position, Piece[] pieces)
    {
        return pieces.Any(p => p.CurrentPosition == position);
    }

    private Vector2[] GetKingMoves(Piece king, Piece[] board)
    {
        // TODO king can step on its own pieces
        return new List<Vector2>()
        {
            king.CurrentPosition + Vector2.Up,
            king.CurrentPosition + Vector2.Down,
            king.CurrentPosition + Vector2.Left,
            king.CurrentPosition + Vector2.Right,
            king.CurrentPosition + Vector2.Up + Vector2.Right,
            king.CurrentPosition + Vector2.Up + Vector2.Left,
            king.CurrentPosition + Vector2.Down + Vector2.Right,
            king.CurrentPosition + Vector2.Down + Vector2.Left,
        }.ToArray();
    }
}