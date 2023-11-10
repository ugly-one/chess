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
		var whitePlayer = Player.WHITE;
		var blackPlayer = Player.BLACK;
		var whiteKing = new Piece(PieceType.King, whitePlayer, new Vector2(0, 0));
		var blackKing = new Piece(PieceType.King, blackPlayer, new Vector2(5, 5));
		var blackRock = new Piece(PieceType.Rock, blackPlayer, new Vector2(0, 5));
		var whiteRock = new Piece(PieceType.Rock, whitePlayer, new Vector2(2, 3));

		var blackRockUI = pieceFactory.CreatePiece(blackRock, blackPlayer.GetTexture("rock"));
		var whiteRockUI = pieceFactory.CreatePiece(whiteRock, whitePlayer.GetTexture("rock"));

		var whiteKingUI = pieceFactory.CreatePiece(whiteKing, whitePlayer.GetTexture("king"));
		var blackKingUI = pieceFactory.CreatePiece(blackKing, blackPlayer.GetTexture("king"));

		// var (whitePieces, whiteKing) = pieceFactory.CreatePieces(Player.WHITE, 7, 6);
		// var (blackPieces, blackKing) = pieceFactory.CreatePieces(Player.BLACK, 0, 1);
		engine = new Engine();
		foreach (var piece in new [] { whiteKingUI, whiteRockUI })
		{
			AddChild(piece);
			piece.Enable();
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
		}
		
		foreach (var piece in new [] { blackKingUI, blackRockUI})
		{
			AddChild(piece);
			piece.Disable();
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
		}
	}

	private void OnPieceLifted(PieceUI pieceUI)
	{
		GD.Print("EVENT RECEIVED!" + pieceUI.Piece.GetType());
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

		var success = engine.TryMove(droppedPiece.Piece, pieces.Select(p => p.Piece).ToArray(),newPosition);

		if (success)
		{
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
		else
		{
			droppedPiece.CancelMove();
		}
		
		// TODO check each current player's piece and see if there is a possible move to detect a draw
	}
}
