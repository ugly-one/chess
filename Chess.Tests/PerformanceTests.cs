using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Chess.Tests;

public class PerformanceTests
{
    private Dictionary<int, int> depthToCount = new Dictionary<int, int>()
    {
        { 1, 20 },
        { 2, 400 },
        { 3, 8902 },
        { 4, 197281 },
        { 5, 4865609 },
        { 6, 119060324 }
    };

    [Theory]
    [InlineData(5)]
    public void Check(int depth)
    {
        var board = BoardFactory.Default();
        var count = GetPossibleMovesCount(board, 0, depth);
        var expectedCount = 0;
        for (int i = 1; i <= depth; i++)
        {
            expectedCount += depthToCount[i];    
        }
        Assert.Equal(expectedCount, count);
    }

    private int GetPossibleMovesCount(Board board, int currentDepth, int targetDepth)
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
