using System.Collections.Generic;
using Godot;

namespace Chess;

public static class Rock
{
    public static Move[] GetRockMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(board.GetMovesInDirection(piece, Vector2.Down,  piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector2.Left,  piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector2.Up,    piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector2.Right, piece.Color));
        return moves.ToArray();       
    }
}