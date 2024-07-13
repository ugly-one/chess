using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Chess.Tests;

public class BoardTests
{
    [Fact]
    public void KingsCannotAttackEachOther()
    {
        var whiteKing = new Piece(PieceType.King, Color.WHITE, new Vector(0, 0));
        var blackKing = new Piece(PieceType.King, Color.BLACK, new Vector(2, 2));
        var board = new Board(new List<Piece>(){ whiteKing, blackKing });

        var whiteMoves = board.GetPossibleMoves(whiteKing);
        
        Assert.DoesNotContain(whiteMoves, m => m.PieceNewPosition == new Vector(1,1));
    }
    
    /// <summary>
    /// A not moved pawn cannot step on a piece in front of it, nor just over it but it can capture another piece
    /// </summary>
    [Fact]
    public void BasicPawnMoves()
    {
        var whiteKing = new Piece(PieceType.King, Color.WHITE, new Vector(3, 3));
        var blackKing = new Piece(PieceType.King, Color.BLACK, new Vector(7, 7));
        var whitePawn = new Piece(PieceType.Pawn, Color.WHITE, new Vector(3, 4), moved: false);
        var blackPawn = new Piece(PieceType.Pawn, Color.BLACK, new Vector(2, 3));
        var board = new Board(new List<Piece>(){ whiteKing, blackKing, whitePawn, blackPawn });

        var moves = board.GetPossibleMoves(whitePawn);
        
        Assert.Single(moves);
        Assert.Equal(moves.First().PieceNewPosition, new Vector(2,3));
        Assert.Equal(moves.First().PieceToCapture, blackPawn);
    }

    [Fact]
    public void EnPassant()
    {
        var whiteKing = new Piece(PieceType.King, Color.WHITE, new Vector(3, 3));
        var blackKing = new Piece(PieceType.King, Color.BLACK, new Vector(7, 7));
        var whitePawn = new Piece(PieceType.Pawn, Color.WHITE, new Vector(3, 3), moved: true);
        var blackPawn = new Piece(PieceType.Pawn, Color.BLACK, new Vector(2, 1));
        var game = new Game(new List<Piece>(){ whiteKing, blackKing, whitePawn, blackPawn });
        var move = game.TryMove(blackPawn, new Vector(2, 3), null);
        
        var moves = game.Board.GetPossibleMoves(whitePawn);
        
        Assert.Contains(moves, m => m.PieceNewPosition == new Vector(2,2));
        Assert.Contains(moves, m => m.PieceToCapture?.Position == move.PieceNewPosition);
    }
}