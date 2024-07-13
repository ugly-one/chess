using System.Collections.Generic;

namespace Chess;

public static class Bishop
{
    public static Move[] GetBishopMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Up + Vector.Right,   piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Up + Vector.Left,    piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Down + Vector.Left,  piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Down + Vector.Right, piece.Color));
        return moves.ToArray();
    }
}