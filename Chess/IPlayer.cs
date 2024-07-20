using Chess;

public interface IPlayer
{
	MoveWithPromotion GetMove(Board board);
}