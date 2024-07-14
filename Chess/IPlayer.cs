using Chess;

public interface IPlayer
{
	void SetColor(Color color);
	Move GetMove(Board board);
}