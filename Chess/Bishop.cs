using System.Collections.Generic;
using Godot;

namespace Chess;

public static class Bishop
{
    public static Move[] GetBishopMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(board.GetMovesInDirection(piece, Vector2.Up + Vector2.Right,   piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector2.Up + Vector2.Left,    piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector2.Down + Vector2.Left,  piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector2.Down + Vector2.Right, piece.Color));
        return moves.ToArray();
    }
}