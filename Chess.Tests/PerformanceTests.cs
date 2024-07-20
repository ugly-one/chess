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
        { 3, 8902 }
    };

    [Fact]
    public void Depth1()
    {
        var board = new Board();
        var count = GetPossibleMovesCount(board, 0, 1, Color.WHITE);
        Assert.Equal(depthToCount[1], count);
    }

    [Fact]
    public void Depth2()
    {
        var board = new Board();
        var count = GetPossibleMovesCount(board, 0, 2, Color.WHITE);
        Assert.Equal(depthToCount[2] + depthToCount[1], count);
    }

    [Fact]
    public void Depth3()
    {
        var board = new Board();
        var count = GetPossibleMovesCount(board, 0, 3, Color.WHITE);
        Assert.Equal(depthToCount[3] + depthToCount[2] + depthToCount[1], count);
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