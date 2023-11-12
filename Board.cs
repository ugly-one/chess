using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Board
{
    private readonly Piece[] _board;
    private readonly Move _lastMove;

    public Board(ICollection<Piece> board, Move lastMove = null)
    {
        _board = board.ToArray();
        _lastMove = lastMove;
    }
    
    /// <summary>
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    public Board Move(Move move, PieceType promotedPiece = PieceType.Queen)
    {
        var takenPiece = move.PieceToCapture;
        var newBoard = _board.ToList();
        if (takenPiece != null)
        {
            newBoard = newBoard.Where(p => p != takenPiece).ToList();
        }
        var newPiece = move.PieceToMove.Move(move.PieceNewPosition);
        // convert a pawn into something better if necessary
        // this is horrible, I have the same logic in Main
        if (move.PieceToMove.Type == PieceType.Pawn && (move.PieceNewPosition.Y == 0 || move.PieceNewPosition.Y == 7))
        {
            newPiece = new Piece(promotedPiece, move.PieceToMove.Color, move.PieceNewPosition, moved: true);
        }
        newBoard = newBoard.Where(p => p != move.PieceToMove).Append(newPiece).ToList();

        var rockToMove = move.RockToMove;
        if (rockToMove != null)
        {
            var newRock = rockToMove.Move(move.RockNewPosition.Value);
            newBoard = newBoard.Where(p => p != move.RockToMove).Append(newRock).ToList();
        }
        newBoard.Remove(move.PieceToMove);

        return new Board(newBoard, move);
    }
    
    
    public bool IsKingUnderAttack(Color color)
    {
        var king = _board
            .First(k => k.Type == PieceType.King && k.Color == color);
        return IsFieldUnderAttack(king.Position, king.Color.GetOppositeColor());
    }
    
    /// <summary>
    /// Check if given field is under attack by given color's pieces
    /// </summary>
    /// <returns></returns>
    public bool IsFieldUnderAttack(Vector2 field, Color color)
    {
        var pieces = GetPieces(color);
        foreach (var piece in pieces)
        {
            var possibleMoves = GetMoves(piece);
            if (possibleMoves.Any(m => m.PieceNewPosition == field))
            {
                return true;
            }
        }
        return false;
    }

    public Piece[] GetPieces(Color color)
    {
        return _board
            .Where(p => p.Color == color)
            .ToArray();
    }
    
    public Piece[] GetPieces()
    {
        return _board
            .ToArray();
    }
    
    /// <summary>
    /// Get moves for the piece without taking into consideration all the rules
    /// But then we should put the logic somewhere to detect that we can't jump over other pieces.
    /// However, it will be tricky because knights CAN jump over pieces and they do not have a range of movement in straight lines
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    public Move[] GetMoves(Piece piece)
    {
        var moves = piece.Type switch
        {
            PieceType.King => King.GetKingMoves(piece, _board),
            PieceType.Queen => Queen.GetQueenMoves(piece, _board),
            PieceType.Pawn => Pawn.GetPawnMoves(piece, _board, _lastMove),
            PieceType.Bishop => Bishop.GetBishopMoves(piece, _board),
            PieceType.Rock => Rock.GetRockMoves(piece, _board),
            PieceType.Knight => Knight.GetKnightMoves(piece, _board)
        };
        return moves;
    }
}