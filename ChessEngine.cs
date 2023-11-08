using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Bla;

public partial class ChessEngine : Node2D
{
	private Player currentPlayer = Player.WHITE;
	private Node board;
	private Dictionary<ColorRect, Color> highlightedFields = new Dictionary<ColorRect, Color>();
	
	private ColorRect GetField(Vector2 position)
	{
		var number = 8 - position.Y;
		if (number < 1 || number > 8)
		{
			throw new NotSupportedException();
		}
		var letter = position.X switch
		{
			0 => "a",
			1 => "b",
			2 => "c",
			3 => "d",
			4 => "e",
			5 => "f",
			6 => "g",
			7 => "h",
			_ => throw new NotSupportedException()
		};
		return board.GetNode<ColorRect>(letter+number);
	}
	public override void _Ready()
	{
		board = GetNode("board");
		var pieceFactory = new PieceFactory();
		var whitePieces = pieceFactory.CreatePieces(Player.WHITE, 7, 6);
		var blackPieces = pieceFactory.CreatePieces(Player.BLACK, 0, 1);

		foreach (var piece in whitePieces)
		{
			AddChild(piece);
			piece.Enable();
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
		}
		
		foreach (var piece in blackPieces)
		{
			AddChild(piece);
			piece.Disable();
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
		}
	}

	private void OnPieceLifted(Piece piece)
	{
		var pieces = GetChildren().OfType<Piece>().ToArray();

		var possibleMoves = piece
			.Movement.GetMoves(pieces, piece.Movement.CurrentPosition)
			.WithinBoard();
		foreach (var possibleMove in possibleMoves)
		{
			highlightedFields.Add(GetField(possibleMove), GetField(possibleMove).Color);			
			GetField(possibleMove).Color = Colors.Pink;
			GD.Print(possibleMove);
		}
	}
	
	private void PieceOnDropped(Piece droppedPiece, Vector2 currentPosition, Vector2 newPosition)
	{
		// reset the board so nothing is highlighted
		foreach (var (field, color) in highlightedFields)
		{
			field.Color = color;
		}
		highlightedFields.Clear();
		
		var pieces = GetChildren().OfType<Piece>().ToArray();
		var possibleMoves = droppedPiece.Movement
			.GetMoves(pieces, droppedPiece.Movement.CurrentPosition)
			.WithinBoard();
		
		if (!possibleMoves.Contains(newPosition))
		{
			droppedPiece.Move(currentPosition);
			return;
		}
		//
		// // disable dropping pieces if their path to the destination is not clear
		// var path = currentPosition.GetFieldsOnPathTo(newPosition);
		// foreach (var piece in pieces)
		// {
		// 	if (path.Contains(piece.Movement.CurrentPosition))
		// 	{
		// 		droppedPiece.Move(currentPosition);
		// 		return;
		// 	}
		// }
		
		// kill opponents piece if needed
		foreach (var piece in pieces)
		{
			if (piece.Movement.CurrentPosition == newPosition && piece.Player == droppedPiece.Player.GetOppositePlayer())
				piece.QueueFree();
		}
		
		droppedPiece.Move(newPosition);
		currentPlayer = currentPlayer.GetOppositePlayer();

		foreach (var piece in pieces)
		{
			if (piece.Player == currentPlayer)
				piece.Enable();
			else
				piece.Disable();
		}
	}
}
