using System;
using System.Collections.Generic;

namespace Chess.Tests;

public static class Printer
{
    public static void Print(this IEnumerable<Move> moves)
    {
        foreach (var move in moves)
        {
            Console.WriteLine(move);
        }
    }
}
