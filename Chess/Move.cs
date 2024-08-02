namespace Chess;

public record MoveWithPromotion(Piece Piece, Vector PieceOldPosition, Vector Position, PieceType? PromotedPiece);
public record Move(Piece Piece, Vector PieceOldPosition, Vector PieceNewPosition);
