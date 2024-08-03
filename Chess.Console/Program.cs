namespace Chess.Console;

static class Program
{
    static void Main(string[] args)
    {
        var depth = Int16.Parse(args[0]);
        System.Console.WriteLine("Running depth: " + depth);
        Check(depth);
    }
    
    private static void Check(int depth)
    {
        var board = BoardFactory.Default();
        var count = GetPossibleMovesCount(board, 0, depth);
    }

    private static int GetPossibleMovesCount(Board board, int currentDepth, int targetDepth)
    {
        if (currentDepth == targetDepth)
        {
            return 0;
        }
        var moves = board.GetAllPossibleMoves();
        var sum = 0;
        foreach(var move in moves)
        {
            sum += 1;
            var (_, newBoard) = board.TryMove(move);
            sum += GetPossibleMovesCount(newBoard, currentDepth + 1, targetDepth);
        }
        return sum;
    }
}