using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Board
{
    private readonly Piece[] _pieces;
    private readonly Move _lastMove;

    public Board(ICollection<Piece> board, Move lastMove = null)
    {
        _pieces = board.ToArray();
        _lastMove = lastMove;
    }

    public (Board, Move) TryMove(Piece pieceToMove, Vector2 newPosition, PieceType promotedPiece = PieceType.Queen)
    {
        var possibleMoves = GetPossibleMoves(pieceToMove);

        var move = possibleMoves.FirstOrDefault(m => m.PieceNewPosition == newPosition);
        
        if (move is null)
        {
            return (this, null);
        }

        var board = Move(move, promotedPiece);

        var opponentsColor = pieceToMove.Color.GetOppositeColor();
        
        if (board.GetAllPossibleMovesForColor(opponentsColor).Any())
        {
            return (board, move);
        }
        
        if (board.IsKingUnderAttack(opponentsColor))
        {
            GD.Print("CHECK MATE!!");
        }
        else
        {
            GD.Print("DRAW!!");
        }

        return (board, move);
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
            var boardAfterMove = Move(possibleMove);
            if (boardAfterMove.IsKingUnderAttack(piece.Color)) continue;
            if (possibleMove.PieceToMove.Type == PieceType.King)
            {
                if (possibleMove.RockToMove != null)
                {
                    // we're castling
                    var moveVector = possibleMove.PieceNewPosition - possibleMove.PieceToMove.Position;
                    var oneStepVector = moveVector.Abs().Clamp(new Vector2(0,0),new Vector2(1,0));
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

    private List<Move> GetAllPossibleMovesForColor(Color color)
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

    private Board Move(Move move, PieceType promotedPiece = PieceType.Queen)
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
            newPiece = new Piece(promotedPiece, move.PieceToMove.Color, move.PieceNewPosition, moved: true);
        }
        newBoard = newBoard.Where(p => p != move.PieceToMove).Append(newPiece).ToList();

        var rockToMove = move.RockToMove;
        if (rockToMove != null)
        {
            var newRock = rockToMove.Move(move.RockNewPosition.Value);
            newBoard = newBoard.Where(p => p != move.RockToMove).Append(newRock).ToList();
        }

        return new Board(newBoard, move);
    }


    private bool IsKingUnderAttack(Color color)
    {
        var king = _pieces
            .First(k => k.Type == PieceType.King && k.Color == color);
        return IsFieldUnderAttack(king.Position, king.Color.GetOppositeColor());
    }
    
    /// <summary>
    /// Check if given field is under attack by given color's pieces
    /// </summary>
    /// <returns></returns>
    private bool IsFieldUnderAttack(Vector2 field, Color color)
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
    
    public Piece[] GetPieces()
    {
        return _pieces
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
            PieceType.Knight => Knight.GetKnightMoves(piece, _pieces)
        };
        return moves;
    }
}