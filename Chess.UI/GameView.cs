using System;
using System.Linq;
using ChessAI;
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
	private SimpleAI blackAI;
	private SimpleAI whiteAI;
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
		SimpleAI black = null,
		SimpleAI white = null)
	{
		this.game = game;
		this.blackAI = black;
		this.whiteAI = white;
		// make sure we have UI aligned with game state
		var piecesUi = game.Board.GetPieces().Select(p => PieceFactory.CreatePiece(p.Position, p.Color, p.GetTexture()));
		foreach (var piece in piecesUi)
		{
			board.AddChild(piece);
			piece.Dropped += PieceOnDropped;
			piece.Lifted += OnPieceLifted;
			if (piece.Color == game.CurrentPlayer)
			{
				piece.Enable();
			}
			else
			{
				piece.Disable();
			}
		}
		if (whiteAI != null)
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
			var (piece, position, promotedPiece) = whiteAI.GetMove();
			TryMove(piece, position, promotedPiece);
		}
		else if(game.CurrentPlayer == Color.BLACK && (blackAI?.FoundMove ?? false))
		{
			var (piece, position, promotedPiece) = blackAI.GetMove();
			TryMove(piece, position, promotedPiece);
		}
	}

	private bool TryMove(Piece piece, Vector2 position, PieceType? promotedPiece)
	{
		var newMove = game.TryMove(piece, position, promotedPiece);
		if (newMove == null) 
			return false;
		UpdateUi(newMove);

		if (game.State != GameState.InProgress)
		{
			endOfGameLabel.Text = game.State.ToString();
			endOfGameLabel.Show();
			return true;
		}

		if (game.CurrentPlayer == Color.BLACK && blackAI != null)
		{
			blackAI.FindMove(game);
		}
		
		if (game.CurrentPlayer == Color.WHITE && whiteAI != null)
		{
			whiteAI.FindMove(game);
		}
		return true;
	}

	private void OnPieceLifted(PieceUI pieceUi)
	{
		var piece = game.GetPiece(pieceUi.ChessPosition);
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
		var piece = game.GetPiece(pieceUi.ChessPosition);
		promotionBox.Hide();
		var move = game.TryMove(piece, newPosition, promotedPiece: typeAsEnum);
		if (move is null)
		{
			pieceUi.CancelMove();
			return;
		}
		UpdateUi(move);
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

		var pieceToMove = game.GetPiece(droppedPiece.ChessPosition);
		
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

	private void UpdateUi(Move move)
	{
		var pieces = board.GetChildren()
			.OfType<PieceUI>()
			.ToArray();
		var pieceToMove = pieces.First(p => p.ChessPosition == move.PieceToMove.Position);
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
				whiteCapturedPieces.AddChild(textureRect);
			}
			else
			{
				blackCapturedPieces.AddChild(textureRect);
			}

			pieceToCapture.QueueFree();
		}

		pieceToMove.Move(move.PieceNewPosition);
		if (move.PromotedType != null)
		{
			pieceToMove.ChangeTexture(pieceToMove.Color.GetTexture(move.PromotedType.Value.ToString().ToLower()));
		}
		
		if (move.RockToMove != null)
		{
			var rockToMove = pieces.First(p => p.ChessPosition == move.RockToMove.Position);
			rockToMove.Move(move.RockNewPosition.Value);
		}

		// swap current player
		foreach (var piece in pieces)
		{
			if (piece.Color == game.CurrentPlayer)
				piece.Enable();
			else
				piece.Disable();
		}
	}
}
