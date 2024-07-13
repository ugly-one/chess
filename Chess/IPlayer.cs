using Chess;
using Godot;

public interface IPlayer
{
	void FindMove(Game game);
	bool FoundMove { get;}
    // 1. return type could be a record/class
    // 2. I'm not sure I like the fact that we depend on Godot in this project
	(Piece, Vector2, PieceType?) GetMove();
}