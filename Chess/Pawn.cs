using System.Collections.Generic;

namespace Chess;

internal static class Pawn
{
    public static List<Move> GetPawnMoves(Piece piece, Piece[,] board, Move? lastMove)
    {
        var moves = new List<Move>();
        var direction = piece.Color == Color.WHITE ? Vector.Up : Vector.Down;

        // one step forward if not blocked
        var forward = piece.Position + direction;
        if (forward.IsWithinBoard() && !IsBlocked(forward, board))
        {
            moves.Add(new Move(piece, forward));

            // two steps forward if not moved yet and not blocked
            if (!piece.Moved)
            {
                var forward2Steps = piece.Position + direction + direction;
                if (forward2Steps.IsWithinBoard() && !IsBlocked(forward2Steps, board))
                {
                    moves.Add(new Move(piece, forward2Steps));
                }
            }
        }

        // one down/left if there is an opponent's piece
        var takeLeft = piece.Position + Vector.Left + direction;

        if (takeLeft.IsWithinBoard())
        {
            var possiblyCapturedPiece = board[takeLeft.X, takeLeft.Y];
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
        }


        // one down/right if there is an opponent's piece
        var takeRight = piece.Position + Vector.Right + direction;
        if (takeRight.IsWithinBoard())
        {
            var possiblyCapturedPiece = board[takeRight.X, takeRight.Y];
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
        }
        return moves;
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

    private static bool IsBlocked(Vector position, Piece[,] board)
    {
        if (board[position.X, position.Y] != null) return true;
        return false;
    }
}
