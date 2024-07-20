namespace Chess;

public record MoveWithPromotion(Piece Piece, Vector Position, PieceType? PromotedPiece);
public record Move(Piece PieceToMove, Vector PieceNewPosition);