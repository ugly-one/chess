using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chess;

public class Board
{
    private readonly Piece[,] pieces;
    private readonly Vector whiteKing;
    private readonly Vector blackKing;
    private readonly Move? _lastMove;
    private readonly Color currentPlayer;
    private readonly Dictionary<Piece, Move[]> possibleMovesPerPiece;

    public Color CurrentPlayer => currentPlayer;

    public Board(Piece[,] pieces, Color currentColor, Move lastMove, Vector whiteKing, Vector blackKing)
    {
        this.currentPlayer = currentColor;
        this._lastMove = lastMove;
        this.pieces = pieces;
        this.whiteKing = whiteKing;
        this.blackKing = blackKing;
        this.possibleMovesPerPiece = new Dictionary<Piece, Move[]>();
    }

    public Board(IEnumerable<Piece> board, Color currentPlayer = Color.WHITE, Move? lastMove = null)
    {
        this.currentPlayer = currentPlayer;
        // TODO validate given pieces - they could be in invalid positions
        pieces = new Piece[8, 8];
        foreach (var piece in board)
        {
            pieces[piece.Position.X, piece.Position.Y] = piece;
            if (piece.Color == Color.WHITE)
            {
                if (piece.Type == PieceType.King)
                {
                    whiteKing = piece.Position;
                }
            }
            else
            {
                if (piece.Type == PieceType.King)
                {
                    blackKing = piece.Position;
                }
            }

        }
        if (whiteKing is null || blackKing is null) throw new InvalidOperationException();
        _lastMove = lastMove;
        possibleMovesPerPiece = new Dictionary<Piece, Move[]>();
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
                        if (IsFieldUnderAttack(possibleMove.PieceToMove.Position + oneStepVector, possibleMove.PieceToMove.Color.GetOpposite()))
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

    public IEnumerable<Piece> GetPieces()
    {
        foreach (var piece in pieces)
        {
            if (piece != null) yield return piece;
        }
    }

    public List<Move> GetAllPossibleMoves()
    {
        var pieces = GetPieces(currentPlayer);
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
        var movedPiece = move.PieceToMove.Move(move.PieceNewPosition);
        if (move.PieceToMove.Type == PieceType.Pawn && (move.PieceNewPosition.Y == 0 || move.PieceNewPosition.Y == 7))
        {
            if (promotedPiece is null)
                movedPiece = movedPiece with { Type = PieceType.Queen };
            else
                movedPiece = movedPiece with { Type = promotedPiece.Value };
        }

        var newPieces = new Piece[8,8];
        Piece? pieceToRemove = null;
        if (move is Capture capture)
        {
            pieceToRemove = capture.CapturedPiece;
        }
        Piece? rockToMove = null;
        Vector? rockNewPosition = null;
        if (move is Castle castle)
        {
            rockToMove = castle.Rook;
            rockNewPosition = castle.RookPosition;
            newPieces[rockNewPosition.X, rockNewPosition.Y] = rockToMove.Move(rockNewPosition);
        }

        newPieces[movedPiece.Position.X, movedPiece.Position.Y] = movedPiece;
        foreach(var piece in pieces)
        {
            if (piece is null) continue;
            if (pieceToRemove != null && piece.Position == pieceToRemove.Position) continue;
            if (piece == move.PieceToMove) continue;
            if (rockToMove != null && piece == rockToMove) continue;
            newPieces[piece.Position.X, piece.Position.Y] = piece;
        }

        var newWhiteKing = (movedPiece.Type == PieceType.King && currentPlayer == Color.WHITE) ? movedPiece.Position : whiteKing;
        var newBlackKing = (movedPiece.Type == PieceType.King && currentPlayer == Color.BLACK) ? movedPiece.Position : blackKing;

        return new Board(newPieces,
            currentPlayer.GetOpposite(), 
            move,
            newWhiteKing,
            newBlackKing);
    }

    public bool IsKingUnderAttack(Color kingColor)
    {
        Piece king;
        if (kingColor == Color.WHITE)
        {
            king = pieces[whiteKing.X, whiteKing.Y];
        }
        else
        {
            king = pieces[blackKing.X, blackKing.Y];
        }

        var attackerColor = kingColor.GetOpposite();
        // check horizontal/vertical lines to see if there is a Queen or a Rock
        var targets = Rock.GetTargets(king.Position, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y];
            if (targetPiece.Color == attackerColor &&
                (targetPiece.Type == PieceType.Queen ||
                targetPiece.Type == PieceType.Rock))
            {
                return true;
            }
        }
        // check diagonal lines to see if there is a Queen or a Bishop
        targets = Bishop.GetTargets(king.Position, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y];
            if (targetPiece.Color == attackerColor &&
                (targetPiece.Type == PieceType.Queen ||
                targetPiece.Type == PieceType.Bishop))
            {
                return true;
            }
        }
        // check knights
        targets = Knight.GetTargets(king.Position, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y];
            if (targetPiece.Color == attackerColor &&
                targetPiece.Type == PieceType.Knight)
            {
                return true;
            }
        }
        // check king
        targets = King.GetTargets(king.Position, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y];
            if (targetPiece.Color == attackerColor &&
                targetPiece.Type == PieceType.King)
            {
                return true;
            }
        }
        // check pawns
        targets = Pawn.GetTargets(king.Position, attackerColor, pieces);
        foreach (var target in targets)
        {
            var targetPiece = pieces[target.X, target.Y];
            if (targetPiece.Color == attackerColor &&
                targetPiece.Type == PieceType.Pawn)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Check if given field is under attack by given color's pieces
    /// </summary>
    /// <returns></returns>
    private bool IsFieldUnderAttack(Vector field, Color color)
    {
        var oppositeColor = color.GetOpposite();
        foreach (PieceType pieceType in PieceTypeExtension.PieceTypes)
        {
            var pretendPiece = new Piece(pieceType, oppositeColor, field);
            var moves = GetMoves(pretendPiece);
            if (moves.Any(m => m is Capture capture && capture.CapturedPiece.Type == pieceType))
                return true;
        }

        return false;
    }

    private IEnumerable<Piece> GetPieces(Color color)
    {
        foreach(var piece in pieces)
        {
            if (piece != null && piece.Color == color)
            {
                yield return piece;
            }
        }
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
        var stringRepresentation = new StringBuilder();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
                stringRepresentation.Append(pieces[x, y]?.ToString() ?? "\u00B7");

            stringRepresentation.Append('\n');
        }

        return stringRepresentation.ToString();
    }
}
