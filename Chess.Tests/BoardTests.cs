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
        var whitePawn = new Piece(PieceType.Pawn, Color.WHITE, new Vector(3, 4), Moved: false);
        var blackPawn = new Piece(PieceType.Pawn, Color.BLACK, new Vector(2, 3));
        var board = new Board(new List<Piece>(){ whiteKing, blackKing, whitePawn, blackPawn });

        var moves = board.GetPossibleMoves(whitePawn);
        
        Assert.Single(moves);
        Assert.Equal(moves.First().PieceNewPosition, new Vector(2,3));

        var (success, newBoard) = board.TryMove(whitePawn, moves.First().PieceNewPosition);

        Assert.Equal(3, newBoard.GetPieces().Length);
    }

    [Fact]
    public void EnPassant()
    {
        var whiteKing = new Piece(PieceType.King, Color.WHITE, new Vector(3, 3));
        var blackKing = new Piece(PieceType.King, Color.BLACK, new Vector(7, 7));
        var whitePawn = new Piece(PieceType.Pawn, Color.WHITE, new Vector(3, 3), Moved: true);
        var blackPawn = new Piece(PieceType.Pawn, Color.BLACK, new Vector(2, 1));
        var game = new Game(Color.BLACK, whiteKing, blackKing, whitePawn, blackPawn);
        game.TryMove(blackPawn, new Vector(2, 3), null);
        
        var moves = game.Board.GetPossibleMoves(whitePawn);
        
        Assert.Contains(moves, m => m.PieceNewPosition == new Vector(2,2));
    }

    [Fact]
    public void PromotingBishopIsNotPossible()
    {
        var whiteKing = new Piece(PieceType.King, Color.WHITE, new Vector(1, 2));
        var blackKing = new Piece(PieceType.King, Color.BLACK, new Vector(5, 6));
        var whiteBishop = new Piece(PieceType.Bishop, Color.WHITE, new Vector(1, 1));
        var game = new Game(Color.WHITE, whiteKing, blackKing, whiteBishop);

        game.TryMove(whiteBishop, new Vector(7,7), PieceType.Queen);

        // TODO we have a nasty way of getting a piece at given position
        // game.Board.GetPiece(x,y) would be better. Or game.GetPiece(x,y)
        Assert.Equal(PieceType.Bishop, game.Board.GetPieces().FirstOrDefault(p => p.Position == new Vector(7, 7)).Type);
    }

    [Fact]
    public void PromotingToQueen()
    {
        var whiteKing = new Piece(PieceType.King, Color.WHITE, new Vector(3, 7));
        var blackKing = new Piece(PieceType.King, Color.BLACK, new Vector(3, 0));
        var whitePawn = new Piece(PieceType.Pawn, Color.WHITE, new Vector(6, 1), Moved: true);
        var board = new Board(new [] {whiteKing, blackKing, whitePawn});

        var (success, newBoard) = board.TryMove(whitePawn, new Vector(6,0), PieceType.Queen);

        Assert.True(success);
        Assert.Contains(newBoard.GetPieces(), p => p.Type == PieceType.Queen);
    }
}