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

    public Board()
    {
        _pieces = new Piece[32]
        {
            new Piece(PieceType.Rock, Color.WHITE, new Vector(0, 7)),
            new Piece(PieceType.Knight, Color.WHITE, new Vector(1, 7)),
            new Piece(PieceType.Bishop, Color.WHITE, new Vector(2, 7)),
            new Piece(PieceType.Queen, Color.WHITE, new Vector(3, 7)),
            new Piece(PieceType.King, Color.WHITE, new Vector(4, 7)),
            new Piece(PieceType.Bishop, Color.WHITE, new Vector(5, 7)),
            new Piece(PieceType.Knight, Color.WHITE, new Vector(6, 7)),
            new Piece(PieceType.Rock, Color.WHITE, new Vector(7, 7)),

            new Piece(PieceType.Pawn, Color.WHITE, new Vector(0, 6)),
            new Piece(PieceType.Pawn, Color.WHITE, new Vector(1, 6)),
            new Piece(PieceType.Pawn, Color.WHITE, new Vector(2, 6)),
            new Piece(PieceType.Pawn, Color.WHITE, new Vector(3, 6)),
            new Piece(PieceType.Pawn, Color.WHITE, new Vector(4, 6)),
            new Piece(PieceType.Pawn, Color.WHITE, new Vector(5, 6)),
            new Piece(PieceType.Pawn, Color.WHITE, new Vector(6, 6)),
            new Piece(PieceType.Pawn, Color.WHITE, new Vector(7, 6)),

            new Piece(PieceType.Rock, Color.BLACK, new Vector(0, 0)),
            new Piece(PieceType.Knight, Color.BLACK, new Vector(1, 0)),
            new Piece(PieceType.Bishop, Color.BLACK, new Vector(2, 0)),
            new Piece(PieceType.Queen, Color.BLACK, new Vector(3, 0)),
            new Piece(PieceType.King, Color.BLACK, new Vector(4, 0)),
            new Piece(PieceType.Bishop, Color.BLACK, new Vector(5, 0)),
            new Piece(PieceType.Knight, Color.BLACK, new Vector(6, 0)),
            new Piece(PieceType.Rock, Color.BLACK, new Vector(7, 0)),

            new Piece(PieceType.Pawn, Color.BLACK, new Vector(0, 1)),
            new Piece(PieceType.Pawn, Color.BLACK, new Vector(1, 1)),
            new Piece(PieceType.Pawn, Color.BLACK, new Vector(2, 1)),
            new Piece(PieceType.Pawn, Color.BLACK, new Vector(3, 1)),
            new Piece(PieceType.Pawn, Color.BLACK, new Vector(4, 1)),
            new Piece(PieceType.Pawn, Color.BLACK, new Vector(5, 1)),
            new Piece(PieceType.Pawn, Color.BLACK, new Vector(6, 1)),
            new Piece(PieceType.Pawn, Color.BLACK, new Vector(7, 1)),
        };
        _lastMove = null;
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
        var possibleMoves = GetMoves(piece).WithinBoard();

        var possibleMovesAfterFiltering = new List<Move>();
        foreach (var possibleMove in possibleMoves)
        {
            // let's try to make the move and see if the king is under attack, if yes, move is not allowed
            // it doesn't matter what we promote to
            var boardAfterMove = Move(possibleMove, PieceType.Queen);
            if (boardAfterMove.IsKingUnderAttack(piece.Color)) continue;
            if (possibleMove.PieceToMove.Type == PieceType.King)
            {
                // TODO find out if we're castling.
                // checking if king moved more than 1 square is enough but won't work in CHess960 :D
                var isCastleMove = false;
                if (isCastleMove)
                {
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

    /// <summary>
    /// Maybe it would be better if we return List<Board>?
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public List<Move> GetAllPossibleMovesForColor(Color color)
    {
        var pieces = GetPieces(color);
        var allPossibleMoves = new List<Move>();
        foreach (var piece in pieces)
        {
            // try to find possible moves
            var possibleMoves = GetPossibleMoves(piece);
            allPossibleMoves.AddRange(
                possibleMoves.Select(m => new Move(m.PieceToMove, m.PieceNewPosition)));
        }

        return allPossibleMoves;
    }

    public (bool, Board) TryMove(Piece piece, Vector newPosition, PieceType? promotedPiece = null)
    {
        var possibleMoves = GetPossibleMoves(piece);
        var move = possibleMoves.FirstOrDefault(m => m.PieceNewPosition == newPosition);
        if (move == null)
        {
            return (false, this);
        }

        var newBoard = Move(move, promotedPiece);

        return (true, newBoard);
    }
    // This is a bit funny that someone can tell the engine to promote non-pawn pieces
    // and also I can do it for any moves - including moves in the center of the board
    // but, I do not see an obvious way how to prevent it.
    private Board Move(Move move, PieceType? promotedPiece)
    {
        // move the piece
        var movedPiece = move.PieceToMove.Move(move.PieceNewPosition);
        if (move.PieceToMove.Type == PieceType.Pawn && (move.PieceNewPosition.Y == 0 || move.PieceNewPosition.Y == 7))
        {
            if (promotedPiece is null)
                movedPiece = movedPiece with { Type = PieceType.Queen };
            else
                movedPiece = movedPiece with { Type = promotedPiece.Value };
        }
        var newBoard = _pieces
            .Where(p => p != move.PieceToMove)
            .Append(movedPiece);

        if (move is Capture capture)
        {
            newBoard = newBoard
                .Where(piece => piece != capture.CapturedPiece);
        }
        else if (move is Castle castle)
        {
            var newRock = castle.Rook.Move(castle.RookPosition);
            newBoard = newBoard
                .Where(p => p != castle.Rook)
                .Append(newRock);
        }

        return new Board(newBoard.ToArray(), move);
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
