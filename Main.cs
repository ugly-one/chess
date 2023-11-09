using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Chess;

public partial class Main : Node2D
{
	private Player currentPlayer = Player.WHITE;
	private Node board;
	private Dictionary<ColorRect, Color> highlightedFields = new Dictionary<ColorRect, Color>();
	private Engine engine;
	
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
		var pieceFactory = new Chess.PieceFactory();
		var (whitePieces, whiteKing) = pieceFactory.CreatePieces(Player.WHITE, 7, 6);
		var (blackPieces, blackKing) = pieceFactory.CreatePieces(Player.BLACK, 0, 1);
		engine = new Engine(whiteKing, blackKing);
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

	private void OnPieceLifted(PieceUI pieceUI)
	{
		var pieces = GetChildren()
			.OfType<Chess.PieceUI>()
			.Select( ui => ui.Piece)
			.ToArray();

		var piece = pieceUI.Piece;
		var possibleMoves = engine.GetPossibleMoves(piece, pieces);
		
		foreach (var possibleMove in possibleMoves)
		{
			highlightedFields.Add(GetField(possibleMove), GetField(possibleMove).Color);			
			GetField(possibleMove).Color = Colors.Pink;
		}
	}

	private void PieceOnDropped(PieceUI droppedPiece, Vector2 currentPosition, Vector2 newPosition)
	{
		// reset the board so nothing is highlighted
		foreach (var (field, color) in highlightedFields)
		{
			field.Color = color;
		}
		highlightedFields.Clear();
		
		var pieces = GetChildren()
			.OfType<Chess.PieceUI>()
			.ToArray();
		var possibleMoves = engine.GetPossibleMoves(droppedPiece.Piece, pieces.Select(p => p.Piece).ToArray());
		
		if (!possibleMoves.Contains(newPosition))
		{
			droppedPiece.Move(currentPosition);
			return;
		}
		
		// the move is possible
		
		// kill opponents piece if needed
		foreach (var piece in pieces)
		{
			if (piece.Piece.CurrentPosition == newPosition && piece.Piece.Player == droppedPiece.Piece.Player.GetOppositePlayer())
				piece.QueueFree();
		}
		
		// move
		droppedPiece.Move(newPosition);
		droppedPiece.Piece.Moved = true;
		droppedPiece.Piece.CurrentPosition = newPosition;
		
		// swap current player
		currentPlayer = currentPlayer.GetOppositePlayer();
		foreach (var piece in pieces)
		{
			if (piece.Piece.Player == currentPlayer)
				piece.Enable();
			else
				piece.Disable();
		}
	}
}
