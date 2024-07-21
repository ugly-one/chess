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
        { 4, 197281 }
    };

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    //[InlineData(4)] -- commented out for now because it takes more than a minute to calculate it
    public void Depth1(int depth)
    {
        var board = new Board();
        var count = GetPossibleMovesCount(board, 0, depth, Color.WHITE);
        var expectedCount = 0;
        for (int i = 1; i <= depth; i++)
        {
            expectedCount += depthToCount[i];    
        }
        Assert.Equal(expectedCount, count);
    }

    private int GetPossibleMovesCount(Board board, int currentDepth, int targetDepth, Color color)
    {
        if (currentDepth == targetDepth)
        {
            return 0;
        }
        var moves = board.GetAllPossibleMovesForColor(color);
        var sum = moves.Count;
        foreach(var move in moves)
        {
            var (_, newBoard) = board.TryMove(move);
            sum += GetPossibleMovesCount(newBoard, currentDepth + 1, targetDepth, Change(color));
        }
        return sum;
    }

    private Color Change(Color color)
    {
        if (color == Color.WHITE) return Color.BLACK;
        else return Color.WHITE;
    }
}