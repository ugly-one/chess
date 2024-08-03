using BenchmarkDotNet.Attributes;

namespace Chess.Console;

public class Perft
{
    [Benchmark]
    public void Check()
    {
        var board = BoardFactory.Default();
        var count = GetPossibleMovesCount(board, 0, 5);
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
    }}