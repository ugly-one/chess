using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Engine
{
    public bool IsKingUnderAttack(Piece[] pieces, King king)
    {
        var oppositePlayerPieces = pieces.Where(p => p.Player != king.Player);
        foreach (var oppositePlayerPiece in oppositePlayerPieces)
        {
            var possibleMoves =
                oppositePlayerPiece.GetMoves(pieces);
            if (possibleMoves.Contains(king.CurrentPosition))
            {
                GD.Print("KING UNDER FIRE");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Checks possible moves for the given piece
    /// </summary>
    /// <param name="piece">piece for which possible moves will be calculated</param>
    /// <param name="pieces">entire board</param>
    /// <returns></returns>
    public Vector2[] GetPossibleMoves(Piece piece, Piece[] pieces)
    {
        var possibleMoves = piece.GetMoves(pieces)
            .WithinBoard();

        var possibleMovesAfterFiltering = new List<Vector2>();
        foreach (var possibleMove in possibleMoves)
        {
            // let's try to make the move and see if the king is still under attack
            Piece[] piecesAfterMove = Move(pieces, piece, possibleMove);
            // find the position of the king in the new setup,
            // We can't use the member variable because the king may moved after the move we simulate the move
            var king = piecesAfterMove.OfType<King>().First(k => k.Player == piece.Player);
            // if there is still check after this move - filter the move from possibleMoves
            var isUnderAttack = IsKingUnderAttack(piecesAfterMove, king);
            if (!isUnderAttack)
            {
                possibleMovesAfterFiltering.Add(possibleMove);
            }
        }
        
        return possibleMovesAfterFiltering.ToArray();
    }

    private Piece[] Move(Piece[] pieces, Piece piece, Vector2 move)
    {
        var boardCopy = pieces.ToList(); // shallow copy, do not modify pieces!
        boardCopy.Remove(piece);
        var newPiece = piece.Copy();
        newPiece.CurrentPosition = move;
        // simulate taking a piece
        var takenPiece = boardCopy.FirstOrDefault(p => p.CurrentPosition == newPiece.CurrentPosition);
        if (takenPiece != null)
        {
            boardCopy.Remove(takenPiece);
        }
        boardCopy.Add(newPiece);
        return boardCopy.ToArray();
    }
}