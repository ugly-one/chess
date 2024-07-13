namespace Chess;

public record Move(Piece PieceToMove, Vector PieceNewPosition, Piece PieceToCapture, Piece RockToMove,
    Vector? RockNewPosition, PieceType? PromotedType = null)
{
    public static Move RegularMove(Piece pieceToMove, Vector newPosition)
    {
        return new Move(pieceToMove, newPosition, null, null, null);
    }

    public static Move RegularPromotion(Piece pieceToMove, Vector newPosition, PieceType newType)
    {
        return new Move(pieceToMove, newPosition, null, null, null, newType);
    }
    
    public static Move PromotionWithCapture(Piece pieceToMove, Vector newPosition, Piece pieceToCapture, PieceType newType)
    {
        return new Move(pieceToMove, newPosition, pieceToCapture, null, null, newType);
    }
    
    public static Move Capture(Piece pieceToMove, Vector newPosition, Piece pieceToCapture)
    {
        return new Move(pieceToMove, newPosition, pieceToCapture, null, null);
    }

    public static Move Castle(Piece king, Vector kingNewPosition, Piece rock, Vector rockNewPosition)
    {
        return new Move(king, kingNewPosition, null, rock, rockNewPosition);
        
    }
}