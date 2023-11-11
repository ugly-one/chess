using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess;

public class Engine
{
    private Move lastMove;
    public Piece[] board;
    public Engine(List<Piece> board)
    {
        this.board = board.ToArray();
    }

    /// <summary>
    /// Checks possible moves for the given piece
    /// </summary>
    /// <param name="piece">piece for which possible moves will be calculated</param>
    /// <param name="board">entire board</param>
    /// <returns></returns>
    public Move[] GetPossibleMoves(Piece piece)
    {
        var possibleMoves = GetMoves(piece, board)
            .WithinBoard();

        var possibleMovesAfterFiltering = new List<Move>();
        foreach (var possibleMove in possibleMoves)
        {
            // let's try to make the move and see if the king is still under attack
            Piece[] boardAfterMove = Move(board, piece, possibleMove);
            // find the position of the king in the new setup,
            // We can't use the member variable because the king may moved after the move we simulate the move
            var king = GetKing(boardAfterMove, piece.Color);
            // if there is still check after this move - filter the move out from possibleMoves
            var isUnderAttack = IsKingUnderAttack(boardAfterMove, king);
            if (!isUnderAttack)
            {
                possibleMovesAfterFiltering.Add(possibleMove);
            }
        }

        if (piece.Type != PieceType.King)
        {
            return possibleMovesAfterFiltering.ToArray();
        }
        
        // add castle
        var king_ = GetKing(board, piece.Color);
        var kingUnderAttack = IsFieldUnderAttack(board, king_.Position, king_.Color.GetOppositeColor());

        if (kingUnderAttack)
            return possibleMovesAfterFiltering.ToArray();
        
        // short castle
        var shortCastleMove = TryGetCastleMove(king_, board, Vector2.Right, 2);
        if (shortCastleMove != null)
        {
            possibleMovesAfterFiltering.Add(shortCastleMove);
        }

        // long castle
        var longCastleMove = TryGetCastleMove(king_, board, Vector2.Left, 3);
        if (longCastleMove != null)
        {
            possibleMovesAfterFiltering.Add(longCastleMove);
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
        
        var takenPiece = move.PieceToCapture; 
        if (takenPiece != null)
        {
            board = board.Where(p => p != takenPiece).ToArray();
        }

        lastMove = move;

        
        var newPiece = move.PieceToMove.Move(move.PieceNewPosition);
        // convert a pawn into something better if necessary
        // this is horrible, I have the same logic in Main
        if (move.PieceToMove.Type == PieceType.Pawn && (move.PieceNewPosition.Y == 0 || move.PieceNewPosition.Y == 7))
        {
            newPiece = new Piece(promotedPiece, move.PieceToMove.Color, move.PieceNewPosition, moved: true);
        }
        board = board.Where(p => p != move.PieceToMove).Append(newPiece).ToArray();

        var rockToMove = move.RockToMove;
        if (rockToMove != null)
        {
            var newRock = rockToMove.Move(move.RockNewPosition.Value);
            board = board.Where(p => p != move.RockToMove).Append(newRock).ToArray();
        }
        
        // did we manage to check opponent's king?
        var opponentsKing = GetKing(board, move.PieceToMove.Color.GetOppositeColor());
        var isOpponentsKingUnderFire = IsKingUnderAttack(board, opponentsKing);
        if (!isOpponentsKingUnderFire)
        {
            // check that the opponent have a move, if not - draw
            if (GetAllPossibleMovesForColor(board, opponentsKing.Color).Any())
            {
                return move;
            }
            GD.Print("DRAW!!");
            return move;
        }
        // opponent's king is under fine
        GD.Print("KING IS UNDER FIRE AFTER OUR MOVE");
        // did we manage to check-mate?
        if (GetAllPossibleMovesForColor(board, opponentsKing.Color).Any())
        {
            return move;
        }

        GD.Print("CHECK MATE!!");
        return move;
    }

    private List<Move> GetAllPossibleMovesForColor(Piece[] board, Color color)
    {
        var pieces = GetPieces(board, color);
        var allPossibleMoves = new List<Move>();
        foreach (var opponentsPiece in pieces)
        {
            // try to find possible moves
            var possibleMoves = GetPossibleMoves(opponentsPiece);
            allPossibleMoves.AddRange(possibleMoves);
        }

        return allPossibleMoves;
    }

    /// <summary>
    /// Gets the king if given player
    /// </summary>
    /// <returns></returns>
    private static Piece GetKing(Piece[] board, Color color)
    {
        return board
            .First(k => k.Type == PieceType.King && k.Color == color);
    }
    
    private bool IsKingUnderAttack(Piece[] board, Piece king)
    {
        // TODO consider using IsFieldUnderAttack method
        var oppositePlayerPieces = GetOppositeColorPieces(board, king.Color);
        foreach (var oppositePlayerPiece in oppositePlayerPieces)
        {
            var possibleMoves = GetMoves(oppositePlayerPiece, board);
            var possibleCaptures = possibleMoves.Where(m => m.PieceToCapture != null);
            if (possibleCaptures.Any(m => m.PieceToCapture.Type == PieceType.King))
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
    private bool IsFieldUnderAttack(Piece[] board, Vector2 field, Color color)
    {
        var pieces = GetPieces(board, color);
        foreach (var piece in pieces)
        {
            var possibleMoves = GetMoves(piece, board);
            if (possibleMoves.Any(m => m.PieceNewPosition == field))
            {
                return true;
            }
        }
        return false;
    }

    private static Piece[] GetPieces(Piece[] board, Color color)
    {
        return board
            .Where(p => p.Color == color)
            .ToArray();
    }

    private static Piece[] GetOppositeColorPieces(Piece[] board, Color color)
    {
        return board
            .Where(p => p.Color != color)
            .ToArray();
    }
    
    private Piece[] Move(Piece[] board, Piece piece, Move move)
    {
        var boardCopy = board.ToList(); // shallow copy, do not modify pieces!
        boardCopy.Remove(piece);
        var takenPiece = move.PieceToCapture;
        if (takenPiece != null)
        {
            boardCopy.Remove(takenPiece);
        }
        var newPiece = piece.Move(move.PieceNewPosition);
        boardCopy.Add(newPiece);
        return boardCopy.ToArray();
    }
    
    /// <summary>
    /// Get moves for the piece without taking into consideration all the rules
    /// TODO - maybe this should NOT take the board to be clear that it gives only moves that in theory a piece could do
    /// But then we should put the logic somewhere to detect that we can't jump over other pieces.
    /// However, it will be tricky because knights CAN jump over pieces and they do not have a range of movement in straight lines
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    private Move[] GetMoves(Piece piece, Piece[] board)
    {
        var moves = piece.Type switch
        {
            PieceType.King => GetKingMoves(piece, board),
            PieceType.Queen => GetQueenMoves(piece, board),
            PieceType.Pawn => GetPawnMoves(piece, board),
            PieceType.Bishop => GetBishopMoves(piece, board),
            PieceType.Rock => GetRockMoves(piece, board),
            PieceType.Knight => GetKnightMoves(piece, board)
        };
        return moves;
    }

    private Move[] GetQueenMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Right,   board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Left,    board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Left,  board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Right, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up, board,    piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down, board,  piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Left, board,  piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Right, board, piece.Color));
        return moves.ToArray();
    }

    private Move[] GetRockMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Left, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Right, board, piece.Color));
        return moves.ToArray();
    }

    private Move[] GetBishopMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Right, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Up + Vector2.Left, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Left, board, piece.Color));
        moves.AddRange(GetMovesInDirection(piece, Vector2.Down + Vector2.Right, board, piece.Color));
        return moves.ToArray();
    }
    
    private Move[] GetPawnMoves(Piece piece, Piece[] board)
    {
        var moves = new List<Move>();
        var direction = piece.Color == Color.WHITE ? Vector2.Up : Vector2.Down;
        
        // one step forward if not blocked
        var forward = piece.Position + direction;
        if (!IsBlocked(board, forward))
        {
            moves.Add(Chess.Move.RegularMove(piece, forward));
        }
        
        var opponentsPieces = GetOppositeColorPieces(board, piece.Color);
        // one down/left if there is an opponent's piece
        var takeLeft = piece.Position + Vector2.Left + direction;
        var opponentCapturedPiece = GetPieceInPosition(opponentsPieces, takeLeft);
        if (opponentCapturedPiece != null)
        {
            moves.Add(Chess.Move.Capture(piece, takeLeft, opponentCapturedPiece));
        }
        else
        {
            // TODO This is probably a bug - we shouldn't be using lastMove here
            // GetPawnMoves is executed when we simulate moves and that means lastMove is not updated correctly.
            // We don't update it when we simulate moves
            // check en-passant
            if (lastMove != null)
            {
                var isPawn = lastMove.PieceToMove.Type == PieceType.Pawn;
                var is2StepMove = (lastMove.PieceToMove.Position - lastMove.PieceNewPosition).Abs() == Vector2.Down * 2;
                var isThePawnNowNextToUs = (lastMove.PieceNewPosition - piece.Position) == Vector2.Left;
                if ( isPawn && // was it a pawn
                    is2StepMove && // was it a 2 step move
                    isThePawnNowNextToUs) // was it move next to us 
                {
                    moves.Add(Chess.Move.Capture(piece, takeLeft, GetPieceInPosition(opponentsPieces, lastMove.PieceNewPosition)));                
                }
            }
        }
        
        // one down/right if there is an opponent's piece
        var takeRight = piece.Position + Vector2.Right + direction;
        var opponentCapturedPiece2 = GetPieceInPosition(opponentsPieces, takeRight);
        if (opponentCapturedPiece2 != null)
        {
            moves.Add(Chess.Move.Capture(piece, takeRight, opponentCapturedPiece2));
        }
        else
        {
            // check en-passant
            if (lastMove != null)
            {
                var isPawn = lastMove.PieceToMove.Type == PieceType.Pawn;
                var is2StepMove = (lastMove.PieceToMove.Position - lastMove.PieceNewPosition).Abs() == Vector2.Down * 2;
                var isThePawnNowNextToUs = (lastMove.PieceNewPosition - piece.Position) == Vector2.Right;
                if ( isPawn && // was it a pawn
                     is2StepMove && // was it a 2 step move
                     isThePawnNowNextToUs) // was it move next to us 
                {
                    moves.Add(Chess.Move.Capture(piece, takeRight, GetPieceInPosition(opponentsPieces, lastMove.PieceNewPosition)));                
                }
            }
        }
        // two steps forward if not moved yet and not blocked
        if (piece.Moved) return moves.ToArray();
        var forward2Steps = piece.Position + direction + direction;
        if (!IsBlocked(board, forward2Steps))
        {
            moves.Add(Chess.Move.RegularMove(piece, forward2Steps));
        }

        return moves.ToArray();
    }

    private Piece GetPieceInPosition(Piece[] pieces, Vector2 position)
    {
        return pieces.FirstOrDefault(p => p.Position == position);
    }
    private bool IsBlocked(Piece[] pieces, Vector2 position)
    {
        return pieces.Any(p => p.Position == position);
    }

    private Move[] GetKingMoves(Piece king, Piece[] board)
    {
        var allPositions = new List<Vector2>()
        {
            king.Position + Vector2.Up,
            king.Position + Vector2.Down,
            king.Position + Vector2.Left,
            king.Position + Vector2.Right,
            king.Position + Vector2.Up + Vector2.Right,
            king.Position + Vector2.Up + Vector2.Left,
            king.Position + Vector2.Down + Vector2.Right,
            king.Position + Vector2.Down + Vector2.Left,
        };

        var allMoves = ConvertToMoves(king, board, allPositions);
        
        return allMoves;
    }

    private Move TryGetCastleMove(Piece king, Piece[] board, Vector2 kingMoveDirection, int rockSteps)
    {
        if (king.Moved)
            return null;
        
        var rock = board
            .Where(p => p.Type == PieceType.Rock)
            .FirstOrDefault(p => p.Position == king.Position + kingMoveDirection * (rockSteps + 1));
        if (rock == null || rock.Moved) 
            return null;
        
        // check that there are no pieces in between king and rock
        // step 1 find fields between king and rock
        var allFieldsInBetweenClean = true;

        for (int i = 1; i <= 2; i++)
        {
            var fieldToCheck = king.Position + kingMoveDirection * i;
            if (IsFieldUnderAttack(board, fieldToCheck, king.Color.GetOppositeColor()) || GetPieceInPosition(board, fieldToCheck) != null)
            {
                allFieldsInBetweenClean = false;
                break;
            }
        }

        if (!allFieldsInBetweenClean) return null;
        
        var rockMoveDirection = kingMoveDirection.Orthogonal().Orthogonal();
        return Chess.Move.Castle(
            king,
            king.Position + kingMoveDirection * 2,
            rock,
            rock.Position + rockMoveDirection * rockSteps);

    }

    private Move[] GetKnightMoves(Piece piece, Piece[] board)
    {
        var allPositions = new List<Vector2>()
        {
            piece.Position + Vector2.Up * 2 + Vector2.Right,
            piece.Position + Vector2.Right * 2 + Vector2.Up,
            piece.Position + Vector2.Right * 2 + Vector2.Down,
            piece.Position + Vector2.Down * 2 + Vector2.Right,
            piece.Position + Vector2.Down * 2 + Vector2.Left,
            piece.Position + Vector2.Left * 2 + Vector2.Down,
            piece.Position + Vector2.Up * 2 + Vector2.Left,
            piece.Position + Vector2.Left * 2 + Vector2.Up,
        };

        var allMoves = ConvertToMoves(piece, board, allPositions);
        return allMoves;
    }

    private Move[] ConvertToMoves(Piece piece, Piece[] board, List<Vector2> allPositions)
    {
        return allPositions.Select(p =>
        {
            var pieceOnTheWay = GetPieceInPosition(board, p);
            if (pieceOnTheWay is null)
            {
                return Chess.Move.RegularMove(piece, p);
            }
            if (pieceOnTheWay.Color != piece.Color)
            {
                return Chess.Move.Capture(piece, p, pieceOnTheWay);
            }
            return null;
        }).Where(m => m != null).ToArray();
    }

    private IEnumerable<Move> GetMovesInDirection(
        Piece piece,
        Vector2 step, 
        Piece[] allPieces,
        Color color)
    {
        var newPos = piece.Position + step;
        var breakAfterAdding = false;
        while (newPos.IsWithinBoard() && !newPos.IsOccupiedBy(color, allPieces))
        {
            // we should pass only opponents pieces to GetPieceInPosition
            var capturedPiece = GetPieceInPosition(allPieces, newPos);
            if (capturedPiece != null)
            {
                breakAfterAdding = true;
                yield return Chess.Move.Capture(piece, newPos, capturedPiece);
                
            }
            else
            {
                yield return Chess.Move.RegularMove(piece, newPos);
            }
            newPos += step;
            if (breakAfterAdding)
            {
                break;
            }
        }
    }
}