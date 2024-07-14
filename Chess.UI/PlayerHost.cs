using System.Threading;
using System.Threading.Tasks;

namespace Chess.UI;

public class PlayerHost
{
    private readonly IPlayer ai;
    private Move move;

    public PlayerHost(IPlayer ai)
    {
        this.ai = ai;
    }

    public bool FoundMove { get; private set; }

    public Move GetMove()
    {
        FoundMove = false;
        return move!;
    }

    public void FindMove(Game game)
    {
        FoundMove = false;
        Task.Run(() =>
        {
            Thread.Sleep(50);
            move = ai.GetMove(game.Board);
            FoundMove = true;
        });
    }
}