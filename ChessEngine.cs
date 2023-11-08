using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Bla;

public partial class ChessEngine : Node2D
{
    private Player currentPlayer = Player.WHITE;

    public override void _Ready()
    {
        var pieceFactory = new PieceFactory();
        var white_pieces = pieceFactory.CreatePieces(Player.WHITE, 0, 1);
        var black_pieces = pieceFactory.CreatePieces(Player.BLACK, 7, 6);

        foreach (var piece in white_pieces)
        {
            AddChild(piece);
            piece.enable();
            piece.Dropped += PieceOnDropped;
        }
        
        foreach (var piece in black_pieces)
        {
            AddChild(piece);
            piece.disable();
            piece.Dropped += PieceOnDropped;
        }
    }

    private void PieceOnDropped(Piece droppedPiece, Vector2 currentPosition, Vector2 newPosition)
    {
        if (!droppedPiece.movement.can_move(newPosition))
        {
            droppedPiece.move(currentPosition);
            return;
        }

        var illegalMove = false;
        var childred = GetChildren();
        var pieces = childred.OfType<Piece>().ToArray();
        
        // disable dropping pieces on top of your own pieces
        foreach (var piece in pieces)
        {
            if (newPosition == piece.movement.current_position && piece.player == droppedPiece.player)
            {
                droppedPiece.move(currentPosition);
                return;
            }
        }
        
        // disable dropping pieces if their path to the destination is not clear
        var path = GetFieldsOnPath(currentPosition, newPosition);
        foreach (var piece in pieces)
        {
            if (path.Contains(piece.movement.current_position))
            {
                droppedPiece.move(currentPosition);
                return;
            }
        }
        
        // kill opponents piece if needed
        foreach (var piece in pieces)
        {
            if (piece.movement.current_position == newPosition && piece.player == GetOppositePlayer(droppedPiece.player))
            {
                piece.QueueFree();
            }
        }
        
        droppedPiece.move(newPosition);

        currentPlayer = GetOppositePlayer(currentPlayer);

        foreach (var piece in pieces)
        {
            if (piece.player == currentPlayer)
            {
                piece.enable();
            }
            else
            {
                piece.disable();
            }
        }
    }

    public Player GetOppositePlayer(Player player)
    {
        return player == Player.BLACK ? Player.WHITE : Player.BLACK;
    }
    
    public Vector2[] GetFieldsOnPath(Vector2 start, Vector2 end)
    {
        var path = new List<Vector2>();

        if ((start - end).Abs() == new Vector2(1, 0) || (start - end).Abs() == new Vector2(0, 1))
        { 
            // if we're moving only one field - no path to check
            return path.ToArray();
        }

        // TODO - we've calculated a diff before, can we re-use it?
        var diff = end - start;

        if (diff.X == 0 || diff.Y == 0)
        {
            var fieldsInBetween = diff.Length();
            var field = start;
            for (var i = 0; i < fieldsInBetween - 1; i++)
            {
                field += diff.Normalized();
                path.Add(field);
            }
            return path.ToArray();
        }

        if (Math.Abs(diff.X) == Math.Abs(diff.Y))
        {
            var xDirection = Math.Sign(diff.X);
            var yDirection = Math.Sign(diff.Y);

            var fieldsInBetween = Math.Abs(start.X - end.X);
            var field = start;
            for (var i = 0; i < fieldsInBetween - 1; i++)
            {
                field += new Vector2(xDirection, yDirection);
                path.Add(field);
            }
            return path.ToArray();
        }
        return path.ToArray();
    }
}