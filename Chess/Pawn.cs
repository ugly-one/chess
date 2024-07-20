using System.Collections.Generic;
using System.Linq;

namespace Chess;

internal static class Pawn
{
    public static Move[] GetPawnMoves(Piece piece, Piece[] board, Move? lastMove)
    {
        var moves = new List<Move>();
        var direction = piece.Color == Color.WHITE ? Vector.Up : Vector.Down;
        
        // one step forward if not blocked
        var forward = piece.Position + direction;
        if (!IsBlocked(forward, board))
        {
            moves.Add(new Move(piece, forward));
            
            // two steps forward if not moved yet and not blocked
            if (!piece.Moved)
            {
                var forward2Steps = piece.Position + direction + direction;
                if (!IsBlocked(forward2Steps, board))
                {
                    moves.Add(new Move(piece, forward2Steps));
                }
            }
        }
        
        // one down/left if there is an opponent's piece
        var takeLeft = piece.Position + Vector.Left + direction;
        var possiblyCapturedPiece = board.GetPieceInPosition(takeLeft);
        if (possiblyCapturedPiece != null && possiblyCapturedPiece.Color != piece.Color)
        {
            moves.Add(new Capture(piece, takeLeft, possiblyCapturedPiece));
        }
        else
        {
            var move = Pawn.TryGetEnPassant(piece, takeLeft, lastMove);
            if (move != null)
            {
                moves.Add(move);
            }
        }
        
        // one down/right if there is an opponent's piece
        var takeRight = piece.Position + Vector.Right + direction;
        possiblyCapturedPiece = board.GetPieceInPosition(takeRight);
        if (possiblyCapturedPiece != null && possiblyCapturedPiece.Color != piece.Color)
        {
            moves.Add(new Capture(piece, takeRight, possiblyCapturedPiece));
        }
        else
        {
            var move = Pawn.TryGetEnPassant(piece, takeRight, lastMove);
            if (move != null)
            {
                moves.Add(move);
            }
        }

        return moves.ToArray();
    }

    private static Move? TryGetEnPassant(Piece piece, Vector capturePosition, Move? lastMove)
    {
        if (lastMove == null) return null;
        
        var isPawn = lastMove.PieceToMove.Type == PieceType.Pawn;
        var is2StepMove = (lastMove.PieceToMove.Position - lastMove.PieceNewPosition).Abs() == Vector.Down * 2;
        var isThePawnNowNextToUs = (lastMove.PieceNewPosition - piece.Position) == new Vector((capturePosition - piece.Position).X, 0);
        if (isPawn && // was it a pawn
            is2StepMove && // was it a 2 step move
            isThePawnNowNextToUs) // was it move next to us 
        {
            var pieceToCapture = lastMove.PieceToMove.Move(lastMove.PieceNewPosition);
            return new Capture(piece, capturePosition, pieceToCapture);
        }

        return null;
    }
        
    private static bool IsBlocked(Vector position, Piece[] board)
    {
        return board.Any(p => p.Position == position);
    }
}
