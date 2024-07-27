using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess;

internal static class PieceTypeExtension
{
    public static PieceType[] PieceTypes = (PieceType[])Enum.GetValues(typeof(PieceType));
}

public class Board
{
    private readonly List<Piece> whitePieces;
    private readonly List<Piece> blackPieces;
    private readonly Piece[,] pieces;
    private readonly Vector whiteKing;
    private readonly Vector blackKing;
    private readonly Move? _lastMove;

    private readonly Dictionary<Piece, Move[]> possibleMovesPerPiece;

    public Board(IEnumerable<Piece> board, Move? lastMove = null)
    {
        // TODO validate given pieces - they could be in invalid positions
        pieces = new Piece[8, 8];
        whitePieces = new List<Piece>(16);
        blackPieces = new List<Piece>(16);
        foreach (var piece in board)
        {
            pieces[piece.Position.X, piece.Position.Y] = piece;
            if (piece.Color == Color.WHITE)
            {
                whitePieces.Add(piece);
                if (piece.Type == PieceType.King)
                {
                    whiteKing = piece.Position;
                }
            }
            else
            {
                blackPieces.Add(piece);
                if (piece.Type == PieceType.King)
                {
                    blackKing = piece.Position;
                }
            }

        }
        _lastMove = lastMove;
        possibleMovesPerPiece = new Dictionary<Piece, Move[]>();
    }

    public bool HasInsufficientMatingMaterial()
    {
        if (whitePieces.Count == 1 && blackPieces.Count == 1)
        {
            // only 2 kings left
            return true;
        }

        if (whitePieces.Count == 1)
        {
            if (HasOnlyKingAndBishopOrKnight(blackPieces))
            {
                return true;
            }
        }

        if (blackPieces.Count == 1)
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

    private static bool HasOnlyKingAndBishopOrKnight(List<Piece> pieces)
    {
        return pieces.Count == 2 &&
               pieces.Any(p => p.Type == PieceType.Bishop || p.Type == PieceType.Knight);
    }

    /// <summary>
    /// Checks possible moves for the given piece
    /// </summary>
    /// <param name="piece">piece for which possible moves will be calculated</param>
    /// <returns></returns>
    public Move[] GetPossibleMoves(Piece piece)
    {
        if (possibleMovesPerPiece.TryGetValue(piece, out var possibleMoves))
        {
            return possibleMoves;
        }
        else
        {
            var possibleMoves2 = GetMoves(piece).WithinBoard();
            var possibleMovesAfterFiltering = new List<Move>();
            foreach (var possibleMove in possibleMoves2)
            {
                // let's try to make the move and see if the king is under attack, if yes, move is not allowed
                // it doesn't matter what we promote to
                var boardAfterMove = Move(possibleMove, PieceType.Queen);
                if (boardAfterMove.IsKingUnderAttack(piece.Color)) continue;
                if (possibleMove.PieceToMove.Type == PieceType.King)
                {
                    // TODO find out if we're castling.
                    // checking if king moved more than 1 square is enough but won't work in CHess960 :D
                    var moveVector = (possibleMove.PieceNewPosition - possibleMove.PieceToMove.Position);
                    var isCastleMove = moveVector.Abs().X > 1;
                    if (isCastleMove)
                    {
                        var oneStepVector = moveVector.Clamp(new Vector(-1, -1), new Vector(1, 1));
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

            possibleMovesPerPiece.Add(piece, possibleMovesAfterFiltering.ToArray());

            return possibleMovesAfterFiltering.ToArray();
        }
    }

    public Piece[] GetPieces()
    {
        return whitePieces.Concat(blackPieces).ToArray();
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

    public (bool, Board) TryMove(Move move, PieceType? promotedPiece = null)
    {
        return TryMove(move.PieceToMove, move.PieceNewPosition, promotedPiece);
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

        IEnumerable<Piece> currentColorPieces = move.PieceToMove.Color == Color.WHITE ?
            whitePieces : blackPieces;

        IEnumerable<Piece> oppositeColorPieces = move.PieceToMove.Color == Color.WHITE ?
            blackPieces : whitePieces;

        currentColorPieces = currentColorPieces
            .Where(p => p != move.PieceToMove)
            .Append(movedPiece);

        if (move is Capture capture)
        {
            oppositeColorPieces = oppositeColorPieces
                .Where(piece => piece != capture.CapturedPiece);
        }
        else if (move is Castle castle)
        {
            var newRock = castle.Rook.Move(castle.RookPosition);
            currentColorPieces = currentColorPieces
                .Where(p => p != castle.Rook)
                .Append(newRock);
        }

        return new Board(currentColorPieces.Concat(oppositeColorPieces), move);
    }

    public bool IsKingUnderAttack(Color color)
    {
        Piece king;
        if (color == Color.WHITE)
        {
            king = pieces[whiteKing.X, whiteKing.Y];
        }
        else
        {
            king = pieces[blackKing.X, blackKing.Y];
        }

        return IsFieldUnderAttack(king.Position, king.Color.GetOppositeColor());
    }

    /// <summary>
    /// Check if given field is under attack by given color's pieces
    /// </summary>
    /// <returns></returns>
    private bool IsFieldUnderAttack(Vector field, Color color)
    {
        var oppositeColor = color.GetOppositeColor();
        foreach (PieceType pieceType in PieceTypeExtension.PieceTypes)
        {
            var pretendPiece = new Piece(pieceType, oppositeColor, field);
            var moves = GetMoves(pretendPiece);
            if (moves.Any(m => m is Capture capture && capture.CapturedPiece.Type == pieceType))
                return true;
        }

        return false;
    }

    private List<Piece> GetPieces(Color color)
    {
        if (color == Color.WHITE) return whitePieces;
        else return blackPieces;
    }

    /// <summary>
    /// Get moves for the piece without taking into consideration all the rules
    /// But then we should put the logic somewhere to detect that we can't jump over other pieces.
    /// However, it will be tricky because knights CAN jump over pieces and they do not have a range of movement in straight lines
    /// </summary>
    /// <param name="piece"></param>
    /// <returns></returns>
    private IEnumerable<Move> GetMoves(Piece piece)
    {
        var moves = piece.Type switch
        {
            PieceType.King => King.GetKingMoves(piece, pieces),
            PieceType.Queen => Queen.GetQueenMoves(piece, pieces),
            PieceType.Pawn => Pawn.GetPawnMoves(piece, pieces, _lastMove),
            PieceType.Bishop => Bishop.GetBishopMoves(piece, pieces),
            PieceType.Rock => Rock.GetRockMoves(piece, pieces),
            PieceType.Knight => Knight.GetKnightMoves(piece, pieces),
            _ => throw new ArgumentOutOfRangeException()
        };
        return moves;
    }

    public override string ToString()
    {
        var boardMatrix = new Piece?[8, 8];
        foreach (var piece in whitePieces.Concat(blackPieces))
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
}
