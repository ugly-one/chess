using ChessAI;
using Godot;

namespace Chess.UI;

public partial class Main : Node2D
{
	// Game state
	private Game? _game;
	private Node? _board;
	
	// UI components
	// I think I need better fields, something with methods: Highlight(), Reset(), so we don't have to keep track of it here
	// Plus, I think it might be good if the pieces will be as children of the fields and not as siblings as it is now.
	private Godot.Collections.Dictionary<ColorRect, Godot.Color> _highlightedFields = new Godot.Collections.Dictionary<ColorRect, Godot.Color>();
	private Button? _newGameButton;
	private Label? _gameStateLabel;
	private HBoxContainer? _whiteCapturedPieces;
	private HBoxContainer? _blackCapturedPieces;
	private PromotionBox? _promotionBox;
	
	//
	private SimpleAI? _blackPlayer;
	private SimpleAI? _whitePlayer;
	
	public override void _Ready()
	{
		_board = GetNode("%board");
		_newGameButton = GetNode<Button>("%newGameButton");
		_newGameButton.Pressed += OnNewGameButtonPressed;
		_gameStateLabel = GetNode<Label>("%gameStateLabel");
		_whiteCapturedPieces = GetNode<HBoxContainer>("%whiteCapturedPieces");
		_blackCapturedPieces = GetNode<HBoxContainer>("%blackCapturedPieces");
		_promotionBox = GetNode<PromotionBox>("%promotionBox");
		_promotionBox.PieceForPromotionSelected += OnPromotionSelected;
		_promotionBox.Hide();
	}

	public override void _Process(double delta)
	{
		if (_game is null) return;

		if (_game.CurrentPlayer == Color.WHITE)
		{
			if (_whitePlayer.FoundMove)
			{
				var (piece, position, promotedPiece) = _whitePlayer.GetMove();
				MoveAndUpdateUi(piece, position, promotedPiece);
				if (_game.State == GameState.InProgress)
				{
					_blackPlayer.FindMove(_game);
				}
			}
		}
		else
		{
			if (_blackPlayer.FoundMove)
			{
				var (piece, position, promotedPiece) = _blackPlayer.GetMove();
				MoveAndUpdateUi(piece, position, promotedPiece);
				if (_game.State == GameState.InProgress)
				{
					_whitePlayer.FindMove(_game);
				}
			}
		}
	}

	private void MoveAndUpdateUi(Piece piece, Vector2 position, PieceType? promotedPiece)
	{
		var newMove = _game.TryMove(piece, position, promotedPiece);
		var pieces = _board.GetChildren()
			.OfType<PieceUI>()
			.ToArray();
		var pieceUiToMove = pieces.First(p => p.ChessPosition == piece.Position);
		UpdateUi(pieceUiToMove, pieces, newMove);
	}

	private void OnNewGameButtonPressed()
	{
		CleanUpCurrentGame();
		SetupNewGame();
	}

	private void CleanUpCurrentGame()
	{
		foreach (var piece in _board.GetChildren().OfType<PieceUI>())
		{
			piece.Dropped -= PieceOnDropped;
			piece.Lifted -= OnPieceLifted;
			piece.QueueFree();
		}
	}
	
	private void SetupNewGame()
	{
		_blackPlayer = new SimpleAI();
		_whitePlayer = new SimpleAI();
		
		var allPieces = PieceFactory.CreateNewGame();

		_game = new Game(allPieces);

		var piecesUi = _game.Board.GetPieces().Select(p => PieceFactory.CreatePiece(p.Position, p.Color, p.GetTexture()));

		foreach (var piece in piecesUi)
		{
			_board.AddChild(piece);
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
			if (piece.Color == _game.CurrentPlayer)
			{
				piece.Enable();
			}
			else
			{
				piece.Disable();
			}
		}

		foreach (var capturedPiece in _whiteCapturedPieces.GetChildren())
		{
			capturedPiece.QueueFree();
		}
		foreach (var capturedPiece in _blackCapturedPieces.GetChildren())
		{
			capturedPiece.QueueFree();
		}
		
		_whitePlayer.FindMove(_game);
	}

	private void OnPieceLifted(PieceUI pieceUi)
	{
		var piece = _game.GetPiece(pieceUi.ChessPosition);
		var possibleMoves = _game.Board.GetPossibleMoves(piece);
		
		foreach (var possibleMove in possibleMoves)
		{
			_highlightedFields.Add(GetField(possibleMove.PieceNewPosition), GetField(possibleMove.PieceNewPosition).Color);			
			GetField(possibleMove.PieceNewPosition).Color = Colors.Pink;
		}
	}
	
	private ColorRect GetField(Vector2 position)
	{
		return _board.GetNode<ColorRect>(position.ToChessNotation());
	}
	
	private void OnPromotionSelected(PieceUI pieceUi, Vector2 newPosition, string type)
	{
		var typeAsEnum = Enum.Parse<PieceType>(type);
		var pieces = _board.GetChildren()
			.OfType<PieceUI>()
			.ToArray();
		var piece = _game.GetPiece(pieceUi.ChessPosition);
		pieceUi.ChangeTexture(pieceUi.Color.GetTexture(type.ToLower()));
		_promotionBox.Hide();
		var move = _game.TryMove(piece, newPosition, promotedPiece: typeAsEnum);
		if (move is null)
		{
			pieceUi.CancelMove();
			return;
		}
		UpdateUi(pieceUi, pieces, move);
	}

	private void PieceOnDropped(PieceUI droppedPiece, Vector2 newPosition)
	{
		// reset the board so nothing is highlighted
		foreach (var (field, color) in _highlightedFields)
		{
			field.Color = color;
		}
		_highlightedFields.Clear();
		
		var pieces = _board.GetChildren()
			.OfType<PieceUI>()
			.ToArray();

		var pieceToMove = _game.GetPiece(droppedPiece.ChessPosition);
		
		if (pieceToMove.Type == PieceType.Pawn &&
			(newPosition.Y == 0 || newPosition.Y == 7))
		{
			var capturedPiece = pieces.FirstOrDefault(p => p.ChessPosition == newPosition && p != droppedPiece);
			capturedPiece?.Hide();
			// do not move the piece yet. It has to stay in place so ProcessDrop can find it
			// I wonder if I go away from having Piece completely decoupled from PieceUI this problem won't occur.
			droppedPiece.SnapToPositionWithoutChangingChessPosition(newPosition);
			_promotionBox.Bla(droppedPiece, newPosition);
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
		UpdateUi(droppedPiece, pieces, move);
	}

	private void UpdateUi(PieceUI droppedPiece, PieceUI[] pieces, Move move)
	{
		// not sure I like the fact that I have to manually update the positions (or kill them) of UI components now
		// it was less code with the events emitted from Piece class (events handled by UI components)
		// but that would mean that Piece class should not be immutable - I think this is a problem because I'm using the previous position of a piece
		// when detecting en-passant
		if (move.PieceToCapture != null)
		{
			var pieceToCapture = pieces.FirstOrDefault(p => p.ChessPosition == move.PieceToCapture.Position);
			var textureRect = new TextureRect()
			{
				Texture = move.PieceToCapture.GetTexture()
			};
			if (move.PieceToCapture.Color == Color.WHITE)
			{
				_whiteCapturedPieces.AddChild(textureRect);
			}
			else
			{
				_blackCapturedPieces.AddChild(textureRect);
			}

			pieceToCapture.QueueFree();
		}

		droppedPiece.Move(move.PieceNewPosition);

		if (move.RockToMove != null)
		{
			var rockToMove = pieces.First(p => p.ChessPosition == move.RockToMove.Position);
			rockToMove.Move(move.RockNewPosition.Value);
		}

		// swap current player
		foreach (var piece in pieces)
		{
			if (piece.Color == _game.CurrentPlayer)
				piece.Enable();
			else
				piece.Disable();
		}
		
		if (_game.State != GameState.InProgress)
		{
			_gameStateLabel.Text = _game.State.ToString();
		}
	}
}
