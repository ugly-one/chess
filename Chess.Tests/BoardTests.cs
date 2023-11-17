using System.Collections.Generic;
using Godot;
using Xunit;

namespace Chess.Tests;

public class BoardTests
{
    [Fact]
    public void KingsCannotAttackEachOther()
    {
        var whiteKing = new Piece(PieceType.King, Color.WHITE, new Vector2(0, 0));
        var blackKing = new Piece(PieceType.King, Color.BLACK, new Vector2(2, 2));
        var board = new Board(new List<Piece>(){ whiteKing, blackKing });

        var whiteMoves = board.GetPossibleMoves(whiteKing);
        
        Assert.DoesNotContain(whiteMoves, m => m.PieceNewPosition == new Vector2(1,1));
    }
}