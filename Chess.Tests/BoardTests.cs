using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Chess.Tests;

public class BoardTests
{
    //var textBoard = new[]
    //{
    ////   01234567
    //    "        ",// 0
    //    "        ",// 1
    //    "        ",// 2
    //    "        ",// 3
    //    "        ",// 4
    //    "        ",// 5
    //    "        ",// 6
    //    "        ",// 7
    //};

    [Fact]
    public void KingsCannotAttackEachOther()
    {
        var board = new[]
        {
        //   01234567
            "K       ",// 0
            "        ",// 1
            "  k     ",// 2
            "        ",// 3
            "        ",// 4
            "        ",// 5
            "        ",// 6
            "        ",// 7
        }.ToBoard();

        var whiteMoves = board.GetAllPossibleMovesForColor(Color.WHITE);

        Assert.DoesNotContain(whiteMoves, m => m.PieceNewPosition == new Vector(1, 1));
    }

    /// <summary>
    /// A not moved pawn cannot step on a piece in front of it, nor just over it but it can capture another piece
    /// </summary>
    [Fact]
    public void BasicPawnMoves()
    {
        var board = new[]
        {
        //   01234567
            "   k    ",// 0
            "        ",// 1
            "        ",// 2
            "  pK    ",// 3
            "   P    ",// 4
            "        ",// 5
            "        ",// 6
            "        ",// 7
        }.ToBoard();
        var whitePawn = board.GetPiece(Color.WHITE, PieceType.Pawn);
        var moves = board.GetPossibleMoves(whitePawn);

        Assert.Single(moves);
        Assert.Equal(moves.First().PieceNewPosition, new Vector(2, 3));

        var (success, newBoard) = board.TryMove(whitePawn, moves.First().PieceNewPosition);

        Assert.Equal(3, newBoard.GetPieces().Length);
    }

    [Fact]
    public void EnPassantIsPossible()
    {
        var board = new[]
        {
        //   01234567
            "   k    ",// 0
            "  p     ",// 1
            "        ",// 2
            "   P    ",// 3
            "        ",// 4
            "        ",// 5
            "        ",// 6
            "   K    ",// 7
        }.ToBoard();
        var blackPawn = board.GetPiece(Color.BLACK, PieceType.Pawn);
        var (success, newBoard) = board.TryMove(blackPawn, new Vector(2, 3));
        Assert.True(success);
        var whitePawn = newBoard.GetPiece(Color.WHITE, PieceType.Pawn);

        var moves = newBoard.GetPossibleMoves(whitePawn);

        Assert.Contains(moves, m => m.PieceNewPosition == new Vector(2, 2));
    }

    [Fact]
    public void PromotingBishopToQueenIsNotPossible()
    {
        var board = new[]
        {
        //   01234567
            "        ",// 0
            " B      ",// 1
            " K      ",// 2
            "        ",// 3
            "        ",// 4
            "        ",// 5
            "     k  ",// 6
            "        ",// 7
        }.ToBoard();

        // this test has to use Game object since promoting logic is a bit in Game class - to be fixed
        var game = new Game(board);
        var bishop = game.Board.GetPiece(PieceType.Bishop);

        game.TryMove(bishop, new Vector(7, 7), PieceType.Queen);

        // TODO we have a nasty way of getting a piece at given position
        // game.Board.GetPiece(x,y) would be better. Or game.GetPiece(x,y)
        Assert.Equal(PieceType.Bishop, game.Board.GetPieces().First(p => p.Position == new Vector(7, 7)).Type);
    }

    [Fact]
    public void PromotingToQueenIsPossible()
    {
        var board = new[]
        {
            "   k    ",
            "        ",
            "        ",
            "        ",
            "        ",
            "       p",
            "   K    ",
        }.ToBoard();
        var pawn = board.GetPiece(PieceType.Pawn);

        var (success, newBoard) = board.TryMove(pawn, new Vector(7, 7), PieceType.Queen);

        Assert.True(success);
        newBoard.GetPiece(PieceType.Queen);
    }

    [Fact]
    public void PinnedPieceIsPinned()
    {
        var board = new[]
        {
            "  K     ",
            "  N     ",
            "        ",
            "  r     ",
            "      k ",
            "        ",
            "        ",
            "        ",
        }.ToBoard();

        var moves = board.GetAllPossibleMovesForColor(Color.WHITE);

        Assert.True(moves.All(m => m.PieceToMove.Type == PieceType.King));
    }

    [Fact]
    public void CastlingIsPossible()
    {
        var board = new[]
        {
        //   01234567
            "   K   R",// 0
            "        ",// 1
            "        ",// 2
            "        ",// 3
            "        ",// 4
            "        ",// 5
            "        ",// 6
            "r  k    ",// 7
        }.ToBoard();

        var whiteMoves = board.GetAllPossibleMovesForColor(Color.WHITE);
        Assert.True(whiteMoves.Any(m => m.PieceToMove.Type == PieceType.King && m.PieceNewPosition == new Vector(5, 0)));
        var blackMoves = board.GetAllPossibleMovesForColor(Color.BLACK);
        Assert.True(blackMoves.Any(m => m.PieceToMove.Type == PieceType.King && m.PieceNewPosition == new Vector(2, 7)));
    }

    private void Print(ICollection<Move> moves)
    {
        foreach (var move in moves)
        {
            Console.WriteLine(move);
        }
    }
}
