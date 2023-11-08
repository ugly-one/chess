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
        var whitePieces = pieceFactory.CreatePieces(Player.WHITE, 0, 1);
        var blackPieces = pieceFactory.CreatePieces(Player.BLACK, 7, 6);

        foreach (var piece in whitePieces)
        {
            AddChild(piece);
            piece.Enable();
            piece.Dropped += PieceOnDropped;
        }
        
        foreach (var piece in blackPieces)
        {
            AddChild(piece);
            piece.Disable();
            piece.Dropped += PieceOnDropped;
        }
    }

    private void PieceOnDropped(Piece droppedPiece, Vector2 currentPosition, Vector2 newPosition)
    {
        if (!droppedPiece.Movement.CanMove(newPosition))
        {
            droppedPiece.Move(currentPosition);
            return;
        }

        var pieces = GetChildren().OfType<Piece>().ToArray();
        
        // disable dropping pieces on top of your own pieces
        foreach (var piece in pieces)
        {
            if (newPosition == piece.Movement.CurrentPosition && piece.Player == droppedPiece.Player)
            {
                droppedPiece.Move(currentPosition);
                return;
            }
        }
        
        // disable dropping pieces if their path to the destination is not clear
        var path = GetFieldsOnPath(currentPosition, newPosition);
        foreach (var piece in pieces)
        {
            if (path.Contains(piece.Movement.CurrentPosition))
            {
                droppedPiece.Move(currentPosition);
                return;
            }
        }
        
        // kill opponents piece if needed
        foreach (var piece in pieces)
        {
            if (piece.Movement.CurrentPosition == newPosition && piece.Player == GetOppositePlayer(droppedPiece.Player))
                piece.QueueFree();
        }
        
        droppedPiece.Move(newPosition);
        currentPlayer = GetOppositePlayer(currentPlayer);

        foreach (var piece in pieces)
        {
            if (piece.Player == currentPlayer)
                piece.Enable();
            else
                piece.Disable();
        }
    }

    private Player GetOppositePlayer(Player player)
    {
        return player == Player.BLACK ? Player.WHITE : Player.BLACK;
    }

    private Vector2[] GetFieldsOnPath(Vector2 start, Vector2 end)
    {
        var path = new List<Vector2>();
        
        var diff = end - start;
        if (diff.Abs() == new Vector2(1, 0) || diff.Abs() == new Vector2(0, 1))
        { 
            // if we're moving only one field - no path to check
            return path.ToArray();
        }

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