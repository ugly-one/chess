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
	private Label gameStateLabel;
	private HBoxContainer whiteCapturedPieces;
	private HBoxContainer blackCapturedPieces;
	private PromotionBox promotionBox;
	
	public override void _Ready()
	{
		board = GetNode("%board");
		newGameButton = GetNode<Button>("%newGameButton");
		newGameButton.Pressed += OnNewGameButtonPressed;
		gameStateLabel = GetNode<Label>("%gameStateLabel");
		whiteCapturedPieces = GetNode<HBoxContainer>("%whiteCapturedPieces");
		blackCapturedPieces = GetNode<HBoxContainer>("%blackCapturedPieces");
		promotionBox = GetNode<PromotionBox>("%promotionBox");
		promotionBox.PieceForPromotionSelected += OnPromotionSelected;
		promotionBox.Hide();
	}

	private void OnNewGameButtonPressed()
	{
		CleanUpCurrentGame();
		SetupNewGame();
	}

	private void CleanUpCurrentGame()
	{
		foreach (var pieceUI in board.GetChildren().OfType<PieceUI>())
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
			return PieceFactory.CreatePiece(p.Position, p.Color, p.GetTexture());
		});

		foreach (var piece in piecesUI)
		{
			board.AddChild(piece);
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

		foreach (var capturedPiece in whiteCapturedPieces.GetChildren())
		{
			capturedPiece.QueueFree();
		}
		foreach (var capturedPiece in blackCapturedPieces.GetChildren())
		{
			capturedPiece.QueueFree();
		}
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
	
	private ColorRect GetField(Vector2 position)
	{
		return board.GetNode<ColorRect>(position.ToChessNotation());
	}
	
	private void OnPromotionSelected(PieceUI pieceUI, Vector2 currentPosition, Vector2 newPosition, string type)
	{
		var typeAsEnum = Enum.Parse<PieceType>(type);
		var pieces = board.GetChildren()
			.OfType<Chess.PieceUI>()
			.ToArray();
		var piece = _board.GetPieces().First(p => p.Position == currentPosition);
		pieceUI.ChangeTexture(pieceUI.Color.GetTexture(type.ToLower()));
		ProcessDrop(pieceUI, newPosition, piece, pieces, typeAsEnum);
		promotionBox.Hide();
	}

	private void PieceOnDropped(PieceUI droppedPiece, Vector2 currentPosition, Vector2 newPosition)
	{
		// reset the board so nothing is highlighted
		foreach (var (field, color) in highlightedFields)
		{
			field.Color = color;
		}
		highlightedFields.Clear();
		
		var pieces = board.GetChildren()
			.OfType<Chess.PieceUI>()
			.ToArray();

		var pieceToMove = _board.GetPieces().First(p => p.Position == currentPosition);
		
		if (pieceToMove.Type == PieceType.Pawn &&
			(newPosition.Y == 0 || newPosition.Y == 7))
		{
			var capturedPiece = pieces.FirstOrDefault(p => p.ChessPosition == newPosition && p != droppedPiece);
			capturedPiece?.Hide();
			// do not move the piece yet. It has to stay in place so ProcessDrop can find it
			// I wonder if I go away from having Piece completely decoupled from PieceUI this problem won't occur.
			droppedPiece.SnapToPositionWithoutChangingChessPosition(newPosition);
			promotionBox.Bla(droppedPiece, currentPosition, newPosition);
			// disable all pieces so it's not possible to make any moves until the promotion is done
			foreach (var piece in pieces)
			{
				piece.Disable();
			}
			return;
		}
		ProcessDrop(droppedPiece, newPosition, pieceToMove, pieces, null);
	}

	private void ProcessDrop(
		PieceUI droppedPiece, 
		Vector2 newPosition, 
		Piece pieceToMove,
		PieceUI[] pieces,
		PieceType? promotedPiece)
	{
		var (newBoard, move, state) = _board.TryMove(pieceToMove, newPosition, promotedPiece);
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
				var textureRect = new TextureRect()
				{
					Texture = move.PieceToCapture.GetTexture()
				};
				if (move.PieceToCapture.Color == Color.WHITE)
				{
					whiteCapturedPieces.AddChild(textureRect);
				}
				else
				{
					blackCapturedPieces.AddChild(textureRect);
				}

				pieceUIToCapture.QueueFree();
			}

			droppedPiece.Move(move.PieceNewPosition);

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

		if (state != GameState.InProgress)
		{
			gameStateLabel.Text = state.ToString();
		}
	}
}
