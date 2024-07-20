namespace Chess;

public record MoveWithPromotion(Piece Piece, Vector Position, PieceType? PromotedPiece);
public record Move(Piece PieceToMove, Vector PieceNewPosition);
internal record Capture(Piece Piece, Vector Position, Piece CapturedPiece) : Move(Piece, Position);
internal record Castle(Piece King, Vector KingPosition, Piece Rook, Vector RookPosition): Move(King, KingPosition);
