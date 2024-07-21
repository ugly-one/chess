using System.Collections.Generic;

namespace Chess;

internal static class Rock
{
    public static ICollection<Move> GetRockMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Down,  piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Left,  piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Up,    piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Right, piece.Color));
        return moves;       
    }
}