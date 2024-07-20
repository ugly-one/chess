using System;
using System.Linq;
using Godot;

namespace Chess.UI;

public partial class GameView : HBoxContainer
{
	// Game state
	private Game game;
	private Node board;
	private bool gamePaused;
	// UI components
	// I think I need better fields, something with methods: Highlight(), Reset(), so we don't have to keep track of it here
	// Plus, I think it might be good if the pieces will be as children of the fields and not as siblings as it is now.
	private Godot.Collections.Dictionary<ColorRect, Godot.Color> highlightedFields = new Godot.Collections.Dictionary<ColorRect, Godot.Color>();
	private GridContainer whiteCapturedPieces;
	private GridContainer blackCapturedPieces;
	private PromotionBox promotionBox;
	private PlayerHost blackAI;
	private PlayerHost whiteAI;
	private Label endOfGameLabel;

	public override void _Ready()
	{
		board = GetNode("%board");
		whiteCapturedPieces = GetNode<GridContainer>("%WhiteCapturedPieces");
		blackCapturedPieces = GetNode<GridContainer>("%BlackCapturedPieces");
		promotionBox = GetNode<PromotionBox>("%PromotionBox");
		promotionBox.PieceForPromotionSelected += OnPromotionSelected;
		promotionBox.Hide();
		endOfGameLabel = GetNode<Label>("%EndOfGameLabel");
		endOfGameLabel.Hide();
	}

	public void StartNewGame(Game game, 
		PlayerHost black = null,
		PlayerHost white = null)
	{
		this.game = game;
		GetNode<AnalysePanel>("analysePanel").Display(game.Board);
		this.blackAI = black;
		this.whiteAI = white;
		// make sure we have UI aligned with game state
		var piecesUi = game.Board.GetPieces().Select(p => PieceFactory.CreatePiece(p.Position.ToVector2(), p.Color, p.GetTexture()));
		foreach (var piece in piecesUi)
		{
			board.AddChild(piece);
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
		}

		if (whiteAI == null)
		{
			EnableColor(Color.WHITE);
		}
		else
		{
			whiteAI.FindMove(game);
		}
	}

	public override void _Process(double delta)
	{
		if (game.State != GameState.InProgress)
		{
			return;
		}
		
		if (game.CurrentPlayer == Color.WHITE && (whiteAI?.FoundMove ?? false))
		{
			var move = whiteAI.GetMove();
			TryMove(move.Piece, move.Position.ToVector2(), move.PromotedPiece);
		}
		else if(game.CurrentPlayer == Color.BLACK && (blackAI?.FoundMove ?? false))
		{
			var move = blackAI.GetMove();
			TryMove(move.Piece, move.Position.ToVector2(), move.PromotedPiece);
		}
	}

	private bool TryMove(Piece piece, Vector2 position, PieceType? promotedPiece)
	{
		var success = game.TryMove(piece, position.ToVector(), promotedPiece);
		if (!success) 
			return false;

		UpdateUi();

		GetNode<AnalysePanel>("analysePanel").Display(game.Board);
		if (game.State != GameState.InProgress)
		{
			endOfGameLabel.Text = game.State.ToString();
			endOfGameLabel.Show();
			return true;
		}

		if (game.CurrentPlayer == Color.BLACK)
		{
			if (blackAI == null)
			{
				EnableColor(Color.BLACK);
			}
			else
			{
				blackAI.FindMove(game);
			}
		} 
		
		if (game.CurrentPlayer == Color.WHITE)
		{
			if (whiteAI == null)
			{
				EnableColor(Color.WHITE);
			}
			else
			{
				whiteAI.FindMove(game);
			}
		}
		return true;
	}

	private void EnableColor(Color color)
	{
		var allPieces = board.GetChildren()
			.OfType<PieceUI>();

		foreach (var piece in allPieces.Where(p => p.Color == color))
		{
			piece.Enable();
		}
		foreach (var piece in allPieces.Where(p => p.Color != color))
		{
			piece.Disable();
		}
	}

	private void OnPieceLifted(PieceUI pieceUi)
	{
		var piece = game.GetPiece(pieceUi.ChessPosition.ToVector());
		var possibleMoves = game.Board.GetPossibleMoves(piece);
		
		foreach (var possibleMove in possibleMoves)
		{
			var field = board.GetNode<ColorRect>(possibleMove.PieceNewPosition.ToChessNotation());
			highlightedFields.Add(field, field.Color);			
			field.Color = Colors.Pink;
		}
	}
	
	private void OnPromotionSelected(PieceUI pieceUi, Vector2 newPosition, string type)
	{
		var typeAsEnum = Enum.Parse<PieceType>(type);
		var piece = game.GetPiece(pieceUi.ChessPosition.ToVector());
		promotionBox.Hide();
		var success = game.TryMove(piece, newPosition.ToVector(), promotedPiece: typeAsEnum);
		if (!success)
		{
			pieceUi.CancelMove();
			return;
		}
		UpdateUi();
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

		var pieceToMove = game.GetPiece(droppedPiece.ChessPosition.ToVector());
		
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
		var moved = TryMove(pieceToMove, newPosition, promotedPiece: null);
		if (!moved)
		{
			droppedPiece.CancelMove();
		}
	}

	private void UpdateUi()
	{
		var pieces = board.GetChildren()
			.OfType<PieceUI>()
			.ToArray();

		foreach (var piece in pieces)
		{
			piece.Dropped -= PieceOnDropped;
			piece.Lifted -= OnPieceLifted;
			piece.QueueFree();
		}

		var piecesUi = game.Board.GetPieces().Select(p => PieceFactory.CreatePiece(p.Position.ToVector2(), p.Color, p.GetTexture()));
		foreach (var piece in piecesUi)
		{
			board.AddChild(piece);
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
		}
	}
}
