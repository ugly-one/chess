using System.Collections.Generic;

namespace Chess;

public static class Rock
{
    public static Move[] GetRockMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Down,  piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Left,  piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Up,    piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Right, piece.Color));
        return moves.ToArray();       
    }
}