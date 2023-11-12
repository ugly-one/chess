using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Game
{
    public Board board;
    
    public Game(List<Piece> board)
    {
        this.board = new Board(board.ToArray());
    }

    /// <summary>
    /// Checks possible moves for the given piece
    /// </summary>
    /// <param name="piece">piece for which possible moves will be calculated</param>
    /// <returns></returns>
    public Move[] GetPossibleMoves(Piece piece)
    {
        var possibleMoves = board.GetMoves(piece)
            .WithinBoard();

        var possibleMovesAfterFiltering = new List<Move>();
        foreach (var possibleMove in possibleMoves)
        {
            // let's try to make the move and see if the king is under attack, if yes, move is not allowed
            var boardAfterMove = board.Move(possibleMove);
            if (boardAfterMove.IsKingUnderAttack(piece.Color)) continue;
            if (possibleMove.PieceToMove.Type == PieceType.King)
            {
                if (possibleMove.RockToMove != null)
                {
                    // we're castling
                    var moveVector = possibleMove.PieceNewPosition - possibleMove.PieceToMove.Position;
                    var oneStepVector = moveVector.Abs().Clamp(new Vector2(0,0),new Vector2(1,0));
                    if (board.IsFieldUnderAttack(possibleMove.PieceToMove.Position + oneStepVector, possibleMove.PieceToMove.Color.GetOppositeColor()))
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
            // also doing castle over an attacked field is not allowed
            
        }

        return possibleMovesAfterFiltering.ToArray();
    }

    public Move TryMove(Piece pieceToMove, Vector2 newPosition, PieceType promotedPiece = PieceType.Queen)
    {
        var possibleMoves = GetPossibleMoves(pieceToMove);

        var move = possibleMoves.FirstOrDefault(m => m.PieceNewPosition == newPosition);
        if (move is null)
        {
            return null;
        }

        board = board.Move(move);
        
        // did we manage to check opponent's king?
        var opponentsKing = board.GetKing(move.PieceToMove.Color.GetOppositeColor());
        var isOpponentsKingUnderFire = board.IsPieceUnderAttack(opponentsKing);
        if (!isOpponentsKingUnderFire)
        {
            // check that the opponent have a move, if not - draw
            if (GetAllPossibleMovesForColor(opponentsKing.Color).Any())
            {
                return move;
            }
            GD.Print("DRAW!!");
            return move;
        }
        // opponent's king is under fine
        GD.Print("KING IS UNDER FIRE AFTER OUR MOVE");
        // did we manage to check-mate?
        if (GetAllPossibleMovesForColor(opponentsKing.Color).Any())
        {
            return move;
        }

        GD.Print("CHECK MATE!!");
        return move;
    }

    private List<Move> GetAllPossibleMovesForColor(Color color)
    {
        var pieces = board.GetPieces(color);
        var allPossibleMoves = new List<Move>();
        foreach (var opponentsPiece in pieces)
        {
            // try to find possible moves
            var possibleMoves = GetPossibleMoves(opponentsPiece);
            allPossibleMoves.AddRange(possibleMoves);
        }

        return allPossibleMoves;
    }

   
}