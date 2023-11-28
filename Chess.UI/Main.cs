using ChessAI;
using Godot;

namespace Chess.UI;

public partial class Main : Node2D
{
	// Game state
	private Color currentColor;
	private Game _game;
	private Node board;
	
	// UI components
	// I think I need better fields, something with methods: Highlight(), Reset(), so we don't have to keep track of it here
	// Plus, I think it might be good if the pieces will be as children of the fields and not as siblings as it is now.
	private Godot.Collections.Dictionary<ColorRect, Godot.Color> highlightedFields = new Godot.Collections.Dictionary<ColorRect, Godot.Color>();
	private Button newGameButton;
	private Label gameStateLabel;
	private HBoxContainer whiteCapturedPieces;
	private HBoxContainer blackCapturedPieces;
	private PromotionBox promotionBox;
	
	//
	private SimpleAI ai;
	
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
		ai = new SimpleAI();
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

		(_game) = new Game(allPieces);

		var piecesUI = _game.Board.GetPieces().Select(p => PieceFactory.CreatePiece(p.Position, p.Color, p.GetTexture()));

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

	private void OnPieceLifted(PieceUI pieceUI)
	{
		var piece = _game.Board.GetPieces().First(p => p.Position == pieceUI.ChessPosition);
		var possibleMoves = _game.Board.GetPossibleMoves(piece);
		
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
	
	private void OnPromotionSelected(PieceUI pieceUI, Vector2 newPosition, string type)
	{
		var typeAsEnum = Enum.Parse<PieceType>(type);
		var pieces = board.GetChildren()
			.OfType<PieceUI>()
			.ToArray();
		var piece = _game.Board.GetPieces().First(p => p.Position == pieceUI.ChessPosition);
		pieceUI.ChangeTexture(pieceUI.Color.GetTexture(type.ToLower()));
		promotionBox.Hide();
		var move = _game.TryMove(piece, newPosition, promotedPiece: typeAsEnum);
		if (move is null)
		{
			pieceUI.CancelMove();
			return;
		}
		ProcessMove(pieceUI, pieces, _game.State, move);
	}

	private void PieceOnDropped(PieceUI droppedPiece, Vector2 newPosition)
	{
		// reset the board so nothing is highlighted
		foreach (var (field, color) in highlightedFields)
		{
			field.Color = color;
		}
		highlightedFields.Clear();
		
		var pieces = board.GetChildren()
			.OfType<PieceUI>()
			.ToArray();

		var pieceToMove = _game.Board.GetPieces().First(p => p.Position == droppedPiece.ChessPosition);
		
		if (pieceToMove.Type == PieceType.Pawn &&
			(newPosition.Y == 0 || newPosition.Y == 7))
		{
			var capturedPiece = pieces.FirstOrDefault(p => p.ChessPosition == newPosition && p != droppedPiece);
			capturedPiece?.Hide();
			// do not move the piece yet. It has to stay in place so ProcessDrop can find it
			// I wonder if I go away from having Piece completely decoupled from PieceUI this problem won't occur.
			droppedPiece.SnapToPositionWithoutChangingChessPosition(newPosition);
			promotionBox.Bla(droppedPiece, newPosition);
			// disable all pieces so it's not possible to make any moves until the promotion is done
			foreach (var piece in pieces)
			{
				piece.Disable();
			}
			return;
		}
		var move  = _game.TryMove(pieceToMove, newPosition, promotedPiece: null);
		if (move is null)
		{
			droppedPiece.CancelMove();
			return;
		}
		ProcessMove(droppedPiece, pieces, _game.State, move);
	}

	private void ProcessMove(
		PieceUI droppedPiece,
		PieceUI[] pieces,
		GameState state,
		Move move)
	{
		UpdateUi(droppedPiece, pieces, move);
		if (state != GameState.InProgress)
		{
			gameStateLabel.Text = state.ToString();
		}
		else
		{
			if (currentColor != Color.BLACK) return;
			
			var (pieceToMove, newPosition) = ai.GetMove(_game.Board);
			var newMove = _game.TryMove(pieceToMove, newPosition, promotedPiece: null);

			var pieceUIToMove = pieces.First(p => p.ChessPosition == pieceToMove.Position);
			ProcessMove(pieceUIToMove, pieces, _game.State, newMove);
		}
	}

	private void UpdateUi(PieceUI droppedPiece, PieceUI[] pieces, Move move)
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
}
