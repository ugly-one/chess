using System.Collections.Generic;

namespace Chess;

internal static class Pawn
{
    private static Vector[] whiteAttackPositions = new Vector[]
    {
        Vector.Up + Vector.Left,
        Vector.Up + Vector.Right
    };

    private static Vector[] blackAttackPositions = new Vector[]
    {
        Vector.Down + Vector.Left,
        Vector.Down + Vector.Right
    };


    public static IEnumerable<Move> GetPawnMoves(Piece piece, Vector position, Piece?[,] board, Move? lastMove)
    {
        var direction = piece.Color == Color.WHITE ? Vector.Up : Vector.Down;

        // one step forward if not blocked
        var forward = position + direction;
        if (forward.IsWithinBoard() && !IsBlocked(forward, board))
        {
            yield return new Move(piece, position, forward);

            // two steps forward if not moved yet and not blocked
            if (!piece.Moved)
            {
                var forward2Steps = position + direction + direction;
                if (forward2Steps.IsWithinBoard() && !IsBlocked(forward2Steps, board))
                {
                    yield return new Move(piece, position, forward2Steps);
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
                yield return new Move(piece, position, takeLeft);
            }
            else
            {
                var move = Pawn.TryGetEnPassant(piece, position, takeLeft, lastMove);
                if (move != null)
                {
                    yield return move;
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
                yield return new Move(piece, position, takeRight);
            }
            else
            {
                var move = Pawn.TryGetEnPassant(piece, position, takeRight, lastMove);
                if (move != null)
                {
                    yield return move;
                }
            }
        }
    }

    private static Move? TryGetEnPassant(Piece piece, Vector currentPosition, Vector capturePosition, Move? lastMove)
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

    private static bool IsBlocked(Vector position, Piece?[,] board)
    {
        if (board[position.X, position.Y] != null) return true;
        return false;
    }

    internal static IEnumerable<Vector> GetTargets(Vector position, Color color, Piece?[,] board)
    {
        // this check is reverted because we want to know if the current position is under pawn's attack
        // maybe this method doesn't belong to Pawn file
        var attackPositions = color == Color.WHITE ? blackAttackPositions : whiteAttackPositions;
        foreach (var pos in attackPositions)
        {
            var newPos = position + pos;
            var target = newPos.GetTargetInPosition(board);
            if (target != null)
                yield return target.Value;
        }
    }
}
