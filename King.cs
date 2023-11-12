using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public static class King
{
    public static Move[] GetKingMoves(Piece king, Piece[] board)
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

        var allMoves = Something.ConvertToMoves(king, allPositions, board).ToList();
        
        // short castle
        var shortCastleMove = TryGetCastleMove(king, Vector2.Right, 2, board);
        if (shortCastleMove != null)
        {
            allMoves.Add(shortCastleMove);
        }

        // long castle
        var longCastleMove = TryGetCastleMove(king, Vector2.Left, 3, board);
        if (longCastleMove != null)
        {
            allMoves.Add(longCastleMove);
        }
        
        return allMoves.ToArray();
    }

    private static Move TryGetCastleMove(Piece king, Vector2 kingMoveDirection, int rockSteps, Piece[] _board)
    {
        if (king.Moved)
            return null;
        
        var rock = _board
            .Where(p => p.Type == PieceType.Rock)
            .FirstOrDefault(p => p.Position == king.Position + kingMoveDirection * (rockSteps + 1));
        if (rock == null || rock.Moved) 
            return null;
        
        var allFieldsInBetweenClean = true;

        for (int i = 1; i <= 2; i++)
        {
            var fieldToCheck = king.Position + kingMoveDirection * i;
            if (_board.GetPieceInPosition(fieldToCheck) != null)
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
}