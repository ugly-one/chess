using System.Collections.Generic;

namespace Chess;

public static class Queen
{
    public static Move[] GetQueenMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Up + Vector.Right,   piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Up + Vector.Left,    piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Down + Vector.Left,  piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Down + Vector.Right, piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Up,       piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Down,   piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Left,   piece.Color));
        moves.AddRange(board.GetMovesInDirection(piece, Vector.Right, piece.Color));
        return moves.ToArray();
    }
}
