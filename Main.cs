using System;
using System.Linq;
using Godot;

namespace Chess;

public partial class Main : Node2D
{
	private Color currentColor;
	private Node board;
	private Godot.Collections.Dictionary<ColorRect, Godot.Color> highlightedFields = new Godot.Collections.Dictionary<ColorRect, Godot.Color>();
	private Board _board;
	private Button newGameButton;
	
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
		newGameButton = GetNode<Button>("newGameButton");
		newGameButton.Pressed += OnNewGameButtonPressed;
		//SetupNewGame();
	}

	private void OnNewGameButtonPressed()
	{
		CleanUpCurrentGame();
		SetupNewGame();
	}

	private void CleanUpCurrentGame()
	{
		foreach (var pieceUI in GetChildren().OfType<PieceUI>())
		{
			pieceUI.Dropped -= PieceOnDropped;
			pieceUI.Lifted -= OnPieceLifted;
			pieceUI.QueueFree();
		}
	}
	
	private void SetupNewGame()
	{
		currentColor = Color.WHITE;
		var allPieces = PieceFactory.CreateNewGame();

		_board = new Board(allPieces);

		var piecesUI = _board.GetPieces().Select(p =>
		{
			return PieceFactory.CreatePiece(p.Position, p.Color, GetTexture(p.Type, p.Color));
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
		var piece = _board.GetPieces().First(p => p.Position == position);
		var possibleMoves = _board.GetPossibleMoves(piece);
		
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

		var pieceToMove = _board.GetPieces().First(p => p.Position == currentPosition);
		var (newBoard, move) = _board.TryMove(pieceToMove, newPosition);
		_board = newBoard;
		if (move != null)
		{
			// not sure I like the fact that I have to manually update the positions (or kill them) of UI components now
			// it was less code with the events emitted from Piece class (events handled by UI components)
			// but that would mean that Piece class should not be immutable - I think this is a problem because I'm using the previous position of a piece
			// when detecting en-passant
			if (move.PieceToCapture != null)
			{
				var pieceUIToCapture = pieces.FirstOrDefault(p => p.ChessPosition == move.PieceToCapture.Position);
				pieceUIToCapture.QueueFree();
			}
			
			var pieceUIToMove = pieces.First(p => p.ChessPosition == currentPosition);
			// this is horrible, I have the same logic in Engine
			if (move.PieceToMove.Type == PieceType.Pawn &&
				(move.PieceNewPosition.Y == 0 || move.PieceNewPosition.Y == 7))
			{
				pieceUIToMove.MoveWithPromotion(move.PieceNewPosition, move.PieceToMove.Color.GetTexture("queen"));
			}
			pieceUIToMove.Move(move.PieceNewPosition);

			if (move.RockToMove != null)
			{
				var rockToMoveUI = pieces.First(p => p.ChessPosition == move.RockToMove.Position);
				rockToMoveUI.Move(move.RockNewPosition.Value);
			}
			
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
	}
}
