using Chess;

namespace ChessAI;

public class SimpleAI
{
    private readonly Color _color;

    public SimpleAI(Color color)
    {
        _color = color;
    }

    public void GetMove(Board board)
    {
        throw new NotImplementedException();
    }
}