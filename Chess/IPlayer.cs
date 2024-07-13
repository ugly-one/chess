using Chess;

public interface IPlayer
{
	void FindMove(Game game);
	bool FoundMove { get;}
    // 1. return type could be a record/class
	(Piece, Vector, PieceType?) GetMove();
}