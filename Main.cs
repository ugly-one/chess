using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Chess;

public partial class Main : Node2D
{
	private Color currentColor;
	private Node board;
	private Dictionary<ColorRect, Godot.Color> highlightedFields = new Dictionary<ColorRect, Godot.Color>();
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
		engine = new Engine();
		board = GetNode("board");
		currentColor = Color.WHITE;
		var pieceFactory = new PieceFactory();
		PieceUI[] whitePieces;
		PieceUI[] blackPieces;
		
		// var whiteKing = new Piece(PieceType.King, Color.WHITE, new Vector2(4, 0));
		// var blackKing = new Piece(PieceType.King, Color.BLACK, new Vector2(4, 2));
		// var blackRock = new Piece(PieceType.Rock, Color.BLACK, new Vector2(7, 1));
		// var whiteRock = new Piece(PieceType.Rock, Color.WHITE, new Vector2(6, 7));
		// var blackRockUI = pieceFactory.CreatePiece(blackRock, Color.BLACK.GetTexture("rock"));
		// var whiteRockUI = pieceFactory.CreatePiece(whiteRock, Color.WHITE.GetTexture("rock"));
		// var whiteKingUI = pieceFactory.CreatePiece(whiteKing, Color.WHITE.GetTexture("king"));
		// var blackKingUI = pieceFactory.CreatePiece(blackKing, Color.BLACK.GetTexture("king"));
		// whitePieces = new[] { whiteKingUI, whiteRockUI };
		// blackPieces = new[] { blackKingUI, blackRockUI };
		
		// whitePieces = pieceFactory.CreatePieces(Color.WHITE, 7, 6);
		// blackPieces= pieceFactory.CreatePieces(Color.BLACK, 0, 1);
		
		var whiteKingUI = pieceFactory.CreatePiece(
			new Piece(PieceType.King, Color.WHITE, new Vector2(0, 0)), 
			Color.WHITE.GetTexture("king"));
		var blackKingUI = pieceFactory.CreatePiece(
			new Piece(PieceType.King, Color.BLACK, new Vector2(7, 7)), 
			Color.BLACK.GetTexture("king"));

		var whitePawnUI = pieceFactory.CreatePiece(
			new Piece(PieceType.Pawn, Color.WHITE, new Vector2(2, 6)),
			Color.WHITE.GetTexture("pawn"));
		var blackPawnUI = pieceFactory.CreatePiece(
			new Piece(PieceType.Pawn, Color.BLACK, new Vector2(3, 4)),
			Color.BLACK.GetTexture("pawn"));
		whitePieces = new[] { whiteKingUI, whitePawnUI };
		blackPieces = new[] { blackKingUI, blackPawnUI };
		
		foreach (var piece in whitePieces.Concat(blackPieces))
		{
			AddChild(piece);
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
			if (piece.Piece.Color == currentColor)
			{
				piece.Enable();
			}
			else
			{
				piece.Disable();
			}
		}
	}

	private void OnPieceLifted(PieceUI pieceUI)
	{
		var board = GetChildren()
			.OfType<Chess.PieceUI>()
			.Select( ui => ui.Piece)
			.ToArray();

		var piece = pieceUI.Piece;
		var possibleMoves = engine.GetPossibleMoves(board, piece);
		
		foreach (var possibleMove in possibleMoves)
		{
			highlightedFields.Add(GetField(possibleMove.PieceNewPosition), GetField(possibleMove.PieceNewPosition).Color);			
			GetField(possibleMove.PieceNewPosition).Color = Colors.Pink;
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

		var board = pieces.Select(p => p.Piece).ToArray();
		var success = engine.TryMove(board, droppedPiece.Piece, newPosition);

		if (success)
		{
			// swap current player
			currentColor = currentColor.GetOppositeColor();
			foreach (var piece in pieces)
			{
				if (piece.Piece.Color == currentColor)
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
