using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess;

public class Board
{
    private readonly Piece[] _pieces;
    private readonly Move? _lastMove;

    public Board(ICollection<Piece> board, Move? lastMove = null)
    {
        // TODO validate given pieces - they could be in invalid positions
        _pieces = board.ToArray();
        _lastMove = lastMove;
    }

    public override string ToString()
    {
        var boardMatrix = new Piece?[8, 8];
        foreach (var piece in _pieces)
        {
            boardMatrix[piece.Position.X, piece.Position.Y] = piece;
        }

        var stringRepresentation = new StringBuilder();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
                stringRepresentation.Append(boardMatrix[x, y]?.ToString() ?? "\u00B7");

            stringRepresentation.Append('\n');
        }

        return stringRepresentation.ToString();
    }

    public bool HasInsufficientMatingMaterial()
    {
        if (_pieces.Length == 2)
        {
            // only 2 kings left
            return true;
        }

        var whitePieces = _pieces.Where(p => p.Color == Color.WHITE).ToArray();
        var blackPieces = _pieces.Where(p => p.Color == Color.BLACK).ToArray();

        if (whitePieces.Length == 1)
        {
            if (HasOnlyKingAndBishopOrKnight(blackPieces))
            {
                return true;
            }
        }

        if (blackPieces.Length == 1)
        {
            if (HasOnlyKingAndBishopOrKnight(whitePieces))
            {
                return true;
            }
        }

        if (HasOnlyKingAndBishopOrKnight(whitePieces) && HasOnlyKingAndBishopOrKnight(blackPieces))
        {
            return true;
        }

        return false;
    }

    private static bool HasOnlyKingAndBishopOrKnight(Piece[] blackPieces)
    {
        return blackPieces.Length == 2 &&
               blackPieces.Any(p => p.Type == PieceType.Bishop || p.Type == PieceType.Knight);
    }

    /// <summary>
    /// Checks possible moves for the given piece
    /// </summary>
    /// <param name="piece">piece for which possible moves will be calculated</param>
    /// <returns></returns>
    public Move[] GetPossibleMoves(Piece piece)
    {
        var possibleMoves = GetMoves(piece)
            .WithinBoard();

        var possibleMovesAfterFiltering = new List<Move>();
        foreach (var possibleMove in possibleMoves)
        {
            // let's try to make the move and see if the king is under attack, if yes, move is not allowed
            // it doesn't matter what we promote to
            var boardAfterMove = Move(possibleMove, PieceType.Queen);
            if (boardAfterMove.IsKingUnderAttack(piece.Color)) continue;
            if (possibleMove.PieceToMove.Type == PieceType.King)
            {
                if (possibleMove.RockToMove != null)
                {
                    // we're castling
                    var moveVector = possibleMove.PieceNewPosition - possibleMove.PieceToMove.Position;
                    var oneStepVector = moveVector.Abs().Clamp(new Vector(0,0),new Vector(1,0));
                    if (IsFieldUnderAttack(possibleMove.PieceToMove.Position + oneStepVector, possibleMove.PieceToMove.Color.GetOppositeColor()))
                    {
                        // castling not allowed
                    }
                    else
                    {
                        possibleMovesAfterFiltering.Add(possibleMove);
                    }
                }
                else
                {
                    possibleMovesAfterFiltering.Add(possibleMove);
                }
            }
            else
            {
                possibleMovesAfterFiltering.Add(possibleMove);
            }
        }

        return possibleMovesAfterFiltering.ToArray();
    }

    public Piece[] GetPieces()
    {
        return _pieces
            .ToArray();
    }

    public List<Move> GetAllPossibleMovesForColor(Color color)
    {
        var pieces = GetPieces(color);
        var allPossibleMoves = new List<Move>();
        foreach (var piece in pieces)
        {
            // try to find possible moves
            var possibleMoves = GetPossibleMoves(piece);
            allPossibleMoves.AddRange(possibleMoves);
        }

        return allPossibleMoves;
    }

    // This is a bit funny that someone can tell the engine to promote non-pawn pieces
    // and also I can do it for any moves - including moves in the center of the board
    // but, I do not see an obvious way how to prevent it.
    public Board Move(Move move, PieceType? promotedPiece)
    {
        var takenPiece = move.PieceToCapture;
        var newBoard = _pieces.ToList();
        if (takenPiece != null)
        {
            newBoard = newBoard.Where(p => p != takenPiece).ToList();
        }
        var newPiece = move.PieceToMove.Move(move.PieceNewPosition);
        // convert a pawn into something better if necessary
        // this is horrible, I have the same logic in Main
        if (move.PieceToMove.Type == PieceType.Pawn && (move.PieceNewPosition.Y == 0 || move.PieceNewPosition.Y == 7))
        {
            newPiece = new Piece(promotedPiece!.Value, move.PieceToMove.Color, move.PieceNewPosition, moved: true);
        }
        newBoard = newBoard.Where(p => p != move.PieceToMove).Append(newPiece).ToList();

        var rockToMove = move.RockToMove;
        if (rockToMove != null)
        {
            var newRock = rockToMove.Move(move.RockNewPosition!);
            newBoard = newBoard.Where(p => p != move.RockToMove).Append(newRock).ToList();
        }

        return new Board(newBoard, move);
    }

    public bool IsKingUnderAttack(Color color)
    {
        var king = _pieces
            .First(k => k.Type == PieceType.King && k.Color == color);
        return IsFieldUnderAttack(king.Position, king.Color.GetOppositeColor());
    }

    /// <summary>
    /// Check if given field is under attack by given color's pieces
    /// </summary>
    /// <returns></returns>
    private bool IsFieldUnderAttack(Vector field, Color color)
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

    private Piece[] GetPieces(Color color)
    {
        return _pieces
            .Where(p => p.Color == color)
            .ToArray();
    }

    /// <summary>
    /// Get moves for the piece without taking into consideration all the rules
    /// But then we should put the logic somewhere to detect that we can't jump over other pieces.
    /// However, it will be tricky because knights CAN jump over pieces and they do not have a range of movement in straight lines
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    private Move[] GetMoves(Piece piece)
    {
        var moves = piece.Type switch
        {
            PieceType.King => King.GetKingMoves(piece, _pieces),
            PieceType.Queen => Queen.GetQueenMoves(piece, _pieces),
            PieceType.Pawn => Pawn.GetPawnMoves(piece, _pieces, _lastMove),
            PieceType.Bishop => Bishop.GetBishopMoves(piece, _pieces),
            PieceType.Rock => Rock.GetRockMoves(piece, _pieces),
            PieceType.Knight => Knight.GetKnightMoves(piece, _pieces),
            _ => throw new ArgumentOutOfRangeException()
        };
        return moves;
    }
}
