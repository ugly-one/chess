using System;
using System.Collections.Generic;

namespace Chess;

public class BoardFactory
{
    public static Board Default()
    {
        var pieces = new (Piece, Vector)[32]
        {
            (new Piece(PieceType.Rock, Color.WHITE), new Vector(0, 7)),
            (new Piece(PieceType.Knight, Color.WHITE), new Vector(1, 7)),
            (new Piece(PieceType.Bishop, Color.WHITE), new Vector(2, 7)),
            (new Piece(PieceType.Queen, Color.WHITE), new Vector(3, 7)),
            (new Piece(PieceType.King, Color.WHITE), new Vector(4, 7)),
            (new Piece(PieceType.Bishop, Color.WHITE), new Vector(5, 7)),
            (new Piece(PieceType.Knight, Color.WHITE), new Vector(6, 7)),
            (new Piece(PieceType.Rock, Color.WHITE), new Vector(7, 7)),

            (new Piece(PieceType.Pawn, Color.WHITE), new Vector(0, 6)),
            (new Piece(PieceType.Pawn, Color.WHITE), new Vector(1, 6)),
            (new Piece(PieceType.Pawn, Color.WHITE), new Vector(2, 6)),
            (new Piece(PieceType.Pawn, Color.WHITE), new Vector(3, 6)),
            (new Piece(PieceType.Pawn, Color.WHITE), new Vector(4, 6)),
            (new Piece(PieceType.Pawn, Color.WHITE), new Vector(5, 6)),
            (new Piece(PieceType.Pawn, Color.WHITE), new Vector(6, 6)),
            (new Piece(PieceType.Pawn, Color.WHITE), new Vector(7, 6)),

            (new Piece(PieceType.Rock, Color.BLACK), new Vector(0, 0)),
            (new Piece(PieceType.Knight, Color.BLACK), new Vector(1, 0)),
            (new Piece(PieceType.Bishop, Color.BLACK), new Vector(2, 0)),
            (new Piece(PieceType.Queen, Color.BLACK), new Vector(3, 0)),
            (new Piece(PieceType.King, Color.BLACK), new Vector(4, 0)),
            (new Piece(PieceType.Bishop, Color.BLACK), new Vector(5, 0)),
            (new Piece(PieceType.Knight, Color.BLACK), new Vector(6, 0)),
            (new Piece(PieceType.Rock, Color.BLACK), new Vector(7, 0)),

            (new Piece(PieceType.Pawn, Color.BLACK), new Vector(0, 1)),
            (new Piece(PieceType.Pawn, Color.BLACK), new Vector(1, 1)),
            (new Piece(PieceType.Pawn, Color.BLACK), new Vector(2, 1)),
            (new Piece(PieceType.Pawn, Color.BLACK), new Vector(3, 1)),
            (new Piece(PieceType.Pawn, Color.BLACK), new Vector(4, 1)),
            (new Piece(PieceType.Pawn, Color.BLACK), new Vector(5, 1)),
            (new Piece(PieceType.Pawn, Color.BLACK), new Vector(6, 1)),
            (new Piece(PieceType.Pawn, Color.BLACK), new Vector(7, 1)),
        };
        return new Board(pieces);
    }

    public static Board FromText(string[] textBoard)
    {
        var pieces = new List<(Piece, Vector)>();
        var rowCount = 0;
        foreach(var row in textBoard)
        {
            var columnCount = 0;
            foreach(var character in row)
            {
                if (character == ' ') 
                {
                    columnCount++;
                    continue;
                }
                var position = new Vector(columnCount, rowCount);
                var color = Char.IsLower(character) ? Color.BLACK : Color.WHITE;
                var piece = Char.ToLower(character) switch
                {
                    'k' => new Piece(PieceType.King, color),
                    'q' => new Piece(PieceType.Queen, color),
                    'n' => new Piece(PieceType.Knight, color),
                    'r' => new Piece(PieceType.Rock, color),
                    'p' => new Piece(PieceType.Pawn, color),
                    'b' => new Piece(PieceType.Bishop, color),
                    _ => throw new NotImplementedException()
                };
                pieces.Add((piece, position));
                columnCount++;
            }
            rowCount++;
        }
        return new Board(pieces);
    }
}

