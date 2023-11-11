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
		board = GetNode("board");
		currentColor = Color.WHITE;
		var pieceFactory = new PieceFactory();
		Piece[] whitePieces;
		Piece[] blackPieces;
		
		// var whiteKing = new Piece(PieceType.King, Color.WHITE, new Vector2(4, 0));
		// var blackKing = new Piece(PieceType.King, Color.BLACK, new Vector2(4, 2));
		// var blackRock = new Piece(PieceType.Rock, Color.BLACK, new Vector2(7, 1));
		// var whiteRock = new Piece(PieceType.Rock, Color.WHITE, new Vector2(6, 7));
		
		whitePieces = pieceFactory.CreatePieces(Color.WHITE, 7, 6);
		blackPieces= pieceFactory.CreatePieces(Color.BLACK, 0, 1);
		engine = new Engine(whitePieces.Concat(blackPieces).ToList());

		var piecesUI = engine.board.Select(p =>
		{
			return pieceFactory.CreatePiece(p.CurrentPosition, p.Color, GetTexture(p.Type, p.Color));
		});
		
		foreach (var piece in piecesUI)
		{
			AddChild(piece);
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
			if (piece.Color == currentColor)
			{
				piece.Enable();
			}
			else
			{
				piece.Disable();
			}
		}
	}

	private Texture2D GetTexture(PieceType argType, Color argColor)
	{
		return argType switch
		{
			PieceType.Bishop => argColor.GetTexture("bishop"),
			PieceType.King => argColor.GetTexture("king"),
			PieceType.Queen => argColor.GetTexture("queen"),
			PieceType.Rock => argColor.GetTexture("rock"),
			PieceType.Pawn => argColor.GetTexture("pawn"),
			PieceType.Knight => argColor.GetTexture("knight"),
		};
	}

	private void OnPieceLifted(PieceUI pieceUI, Vector2 position)
	{
		var piece = engine.board.First(p => p.CurrentPosition == position);
		var possibleMoves = engine.GetPossibleMoves(piece);
		
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

		var pieceToMove = engine.board.First(p => p.CurrentPosition == currentPosition);
		var move = engine.TryMove(pieceToMove, newPosition);

		if (move != null)
		{
			if (move.PieceToCapture != null)
			{
				var pieceUIToCapture = pieces.FirstOrDefault(p => p.ChessPosition == move.PieceToCapture.CurrentPosition);
				pieceUIToCapture.QueueFree();
			}
			
			var pieceUIToMove = pieces.First(p => p.ChessPosition == currentPosition);
			pieceUIToMove.Move(move.PieceNewPosition);
			
			// swap current player
			currentColor = currentColor.GetOppositeColor();
			foreach (var piece in pieces)
			{
				if (piece.Color == currentColor)
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
