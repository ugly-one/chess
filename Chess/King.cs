using System.Collections.Generic;
using System.Linq;

namespace Chess;

internal static class King
{
    public static List<Move> GetKingMoves(Piece king, Piece[,] board)
    {
        var allPositions = new Vector[]
        {
            king.Position + Vector.Up,
            king.Position + Vector.Down,
            king.Position + Vector.Left,
            king.Position + Vector.Right,
            king.Position + Vector.Up + Vector.Right,
            king.Position + Vector.Up + Vector.Left,
            king.Position + Vector.Down + Vector.Right,
            king.Position + Vector.Down + Vector.Left,
        };

        allPositions = allPositions.Where(p => p.IsWithinBoard()).ToArray();
        var allMoves = Something.ConvertToMoves(king, allPositions, board);
        
        // short castle
        var shortCastleMove = TryGetCastleMove(king, Vector.Right, 2, board);
        if (shortCastleMove != null)
        {
            allMoves.Add(shortCastleMove);
        }

        // long castle
        var longCastleMove = TryGetCastleMove(king, Vector.Left, 3, board);
        if (longCastleMove != null)
        {
            allMoves.Add(longCastleMove);
        }
        
        return allMoves;
    }

    private static Move? TryGetCastleMove(Piece king, Vector kingMoveDirection, int rockSteps, Piece[,] _board)
    {
        if (king.Moved)
            return null;
        
        var possibleRockPosition = king.Position + kingMoveDirection * (rockSteps + 1);
        if (!possibleRockPosition.IsWithinBoard())
        {
            // TODO I'm wasting time here. I shouldn't even consider such position. 
            // finding both rocks for king's position should be as simple as doing 2 lookups in the board
            // the position of rocks never change. And if it changed (and we can't find the rock where it should be) - no castling
            // It may change for Chess960, but for now we could have 2 hardcoded positions to check
            return null;
        }
        var rock = _board[possibleRockPosition.X, possibleRockPosition.Y];

        if (rock == null || rock.Moved) 
            return null;
        
        var allFieldsInBetweenClean = true;

        for (int i = 1; i <= 2; i++)
        {
            var fieldToCheck = king.Position + kingMoveDirection * i;
            if (_board[fieldToCheck.X, fieldToCheck.Y] != null)
            {
                allFieldsInBetweenClean = false;
                break;
            }
        }

        if (!allFieldsInBetweenClean) return null;
        
        var rockMoveDirection = kingMoveDirection.Orthogonal().Orthogonal();
        return new Castle(
            king,
            king.Position + kingMoveDirection * 2,
            rock,
            rock.Position + rockMoveDirection * rockSteps);
    }
}
