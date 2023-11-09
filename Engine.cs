using System.Linq;
using Godot;

namespace Chess;

public class Engine
{
    private King whiteKing;
    private King blackKing;

    public Engine(King white, King black)
    {
        whiteKing = white;
        blackKing = black;
    }
    
    public bool IsKingUnderAttack(Piece[] pieces, Player player)
    {
        var oppositePlayerPieces = pieces.Where(p => p.Player != player);
        foreach (var oppositePlayerPiece in oppositePlayerPieces)
        {
            var possibleMoves =
                oppositePlayerPiece.GetMoves(pieces);
            if (player == Player.WHITE)
            {
                if (possibleMoves.Contains(whiteKing.CurrentPosition))
                {
                    GD.Print("WHITE KING UNDER FIRE");
                    return true;
                }
            }
            else
            {
                if (possibleMoves.Contains(blackKing.CurrentPosition))
                {
                    GD.Print("BLACK KING UNDER FIRE");
                    return true;
                }
            }
        }
        return false;
    }

    // public bool Bla(Vector2[] possibleMoves)
    // {
    //     foreach (var possibleMove in possibleMoves)
    //     {
    //         // // if the king is under attack, let's try to make a move and see if the king is still under attack
    //         // Piece[] piecesAfterMove = Move(pieces, pieceToMove, possibleMove);
    //         // // if there is still check after this move - filter the move from possibleMoves
    //         // var isUnderAttack = IsKingUnderAttack(piecesAfterMove, currentPlayer);
    //         // if (isUnderAttack)
    //         // {
    //         // 	// TODO filter out the move
    //         // }
    //     }
    //
    //     return false;
    // }

    public Vector2[] GetPossibleMoves(Piece piece, Piece[] pieces)
    {
        var possibleMoves = piece.GetMoves(pieces)
            .WithinBoard();
        return possibleMoves.ToArray();
    }
}