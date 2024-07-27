namespace Chess.Tests;

public static class StringArrayExtensions
{
    public static Board ToBoard(this string[] board)
    {
        return BoardFactory.FromText(board);
    }
}

