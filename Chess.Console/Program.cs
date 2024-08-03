using Dia2Lib;

namespace Chess.Console;

static class Program
{
    static void Main(string[] args)
    {
        BenchmarkDotNet.Running.BenchmarkRunner.Run<Perft>();
    }
}