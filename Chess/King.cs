using System.Collections.Generic;

namespace Chess;

internal static class King
{
    private static Vector[] positions = new Vector[]
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

    public static IEnumerable<Vector> GetTargets(Vector position, Piece?[,] board)
    {
        foreach (var pos in positions)
        {
            var newPos = position + pos;
            var target = newPos.GetTargetInPosition(board);
            if (target != null)
                yield return target.Value;
        }
    }

    public static IEnumerable<Move> GetKingMoves(Piece king, Vector position, Piece?[,] board)
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

        var allMoves = Something.ConvertToMoves(king, position, allPositions, board);
        foreach (var move in allMoves)
        {
            yield return move;
        }
        // short castle
        var shortCastleMove = TryGetCastleMove(king, position, Vector.Left, 2, board);
        if (shortCastleMove != null)
        {
            yield return shortCastleMove;
        }

        // long castle
        var longCastleMove = TryGetCastleMove(king, position, Vector.Right, 3, board);
        if (longCastleMove != null)
        {
            yield return longCastleMove;
        }
    }

    private static Move? TryGetCastleMove(Piece king, Vector position, Vector kingMoveDirection, int rockSteps, Piece?[,] _board)
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
        var rock = _board[possibleRockPosition.X, possibleRockPosition.Y];

        if (rock == null || rock.Value.Moved)
            return null;

        var allFieldsInBetweenClean = true;

        for (int i = 1; i <= 2; i++)
        {
            var fieldToCheck = position + kingMoveDirection * i;
            if (_board[fieldToCheck.X, fieldToCheck.Y] != null)
            {
                allFieldsInBetweenClean = false;
                break;
            }
        }

        if (!allFieldsInBetweenClean) return null;

        return new Move(king, position, position + kingMoveDirection * 2);
    }
}
