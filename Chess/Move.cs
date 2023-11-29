using Godot;

namespace Chess;

public record Move(Piece PieceToMove, Vector2 PieceNewPosition, Piece PieceToCapture, Piece RockToMove,
    Vector2? RockNewPosition, PieceType? PromotedType = null)
{
    public static Move RegularMove(Piece pieceToMove, Vector2 newPosition)
    {
        return new Move(pieceToMove, newPosition, null, null, null);
    }

    public static Move RegularPromotion(Piece pieceToMove, Vector2 newPosition, PieceType newType)
    {
        return new Move(pieceToMove, newPosition, null, null, null, newType);
    }
    
    public static Move PromotionWithCapture(Piece pieceToMove, Vector2 newPosition, Piece pieceToCapture, PieceType newType)
    {
        return new Move(pieceToMove, newPosition, pieceToCapture, null, null, newType);
    }
    
    public static Move Capture(Piece pieceToMove, Vector2 newPosition, Piece pieceToCapture)
    {
        return new Move(pieceToMove, newPosition, pieceToCapture, null, null);
    }

    public static Move Castle(Piece king, Vector2 kingNewPosition, Piece rock, Vector2 rockNewPosition)
    {
        return new Move(king, kingNewPosition, null, rock, rockNewPosition);
        
    }
}