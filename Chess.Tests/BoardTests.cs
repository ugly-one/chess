using System;
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
        var textBoard = new[]
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
        };


        var board = BoardFactory.FromText(textBoard);

        var whiteMoves = board.GetAllPossibleMovesForColor(Color.WHITE);

        Assert.DoesNotContain(whiteMoves, m => m.PieceNewPosition == new Vector(1, 1));
    }

    /// <summary>
    /// A not moved pawn cannot step on a piece in front of it, nor just over it but it can capture another piece
    /// </summary>
    [Fact]
    public void BasicPawnMoves()
    {
        var textBoard = new[]
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
        };

        var board = BoardFactory.FromText(textBoard);
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
        var textBoard = new[]
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
        };

        var board = BoardFactory.FromText(textBoard);
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
        var textBoard = new[]
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
        };

        // this test has to use Game object since promoting logic is a bit in Game class - to be fixed
        var game = new Game(BoardFactory.FromText(textBoard));
        var bishop = game.Board.GetPiece(PieceType.Bishop);

        game.TryMove(bishop, new Vector(7, 7), PieceType.Queen);

        // TODO we have a nasty way of getting a piece at given position
        // game.Board.GetPiece(x,y) would be better. Or game.GetPiece(x,y)
        Assert.Equal(PieceType.Bishop, game.Board.GetPieces().First(p => p.Position == new Vector(7, 7)).Type);
    }

    [Fact]
    public void PromotingToQueenIsPossible()
    {
        var textBoard = new[]
        {
            "   k    ",
            "        ",
            "        ",
            "        ",
            "        ",
            "       p",
            "   K    ",
        };
        var board = BoardFactory.FromText(textBoard);
        var pawn = board.GetPiece(PieceType.Pawn);

        var (success, newBoard) = board.TryMove(pawn, new Vector(7, 7), PieceType.Queen);

        Assert.True(success);
        newBoard.GetPiece(PieceType.Queen);
    }

    [Fact]
    public void PinnedPieceIsPinned()
    {
        var textBoard = new[]
        {
            "  K     ",
            "  N     ",
            "        ",
            "  r     ",
            "      k ",
            "        ",
            "        ",
            "        ",
        };
        var board = BoardFactory.FromText(textBoard);

        var moves = board.GetAllPossibleMovesForColor(Color.WHITE);

        Assert.True(moves.All(m => m.PieceToMove.Type == PieceType.King));
    }
}
