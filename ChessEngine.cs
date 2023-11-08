using System;
using System.Linq;
using Godot;
using Godot.Collections;

namespace Bla;

public partial class ChessEngine : Node2D
{
	private Player currentPlayer = Player.WHITE;
	private Node board;
	private Dictionary<ColorRect, Color> highlightedFields = new Dictionary<ColorRect, Color>();
	private Vector2 whiteKing;
	private Vector2 blackKing;
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
		var pieceFactory = new PieceFactory();
		var (whitePieces, whiteKing) = pieceFactory.CreatePieces(Player.WHITE, 7, 6);
		var (blackPieces, blackKing) = pieceFactory.CreatePieces(Player.BLACK, 0, 1);
		this.whiteKing = whiteKing;
		this.blackKing = blackKing;
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

	private void OnPieceLifted(Piece piece)
	{
		var pieces = GetChildren().OfType<Piece>().ToArray();

		var possibleMoves = piece
			.Movement.GetMoves(pieces.Select(p => p.Movement).ToArray())
			.WithinBoard()
			.Append(piece.Movement.CurrentPosition);
		
		// check if current player king is under attack
		var bla = IsKingUnderAttack(pieces, currentPlayer);
		
		// if the king is under attack, let's try to make a move and see if the king is still under attack
		
		GD.Print(bla);
		foreach (var possibleMove in possibleMoves)
		{
			highlightedFields.Add(GetField(possibleMove), GetField(possibleMove).Color);			
			GetField(possibleMove).Color = Colors.Pink;
		}
	}

	private bool IsKingUnderAttack(Piece[] pieces, Player player)
	{
		var oppositePlayerPieces = pieces.Where(p => p.Movement.Player != player);
		foreach (var oppositePlayerPiece in oppositePlayerPieces)
		{
			var possibleMoves =
				oppositePlayerPiece.Movement.GetMoves(pieces.Select(p => p.Movement).ToArray());
			if (player == Player.WHITE)
			{
				if (possibleMoves.Contains(whiteKing))
				{
					GD.Print("WHITE KING UNDER FIRE");
					return true;
				}
			}
			else
			{
				if (possibleMoves.Contains(blackKing))
				{
					GD.Print("BLACK KING UNDER FIRE");
					return true;
				}
			}
		}
		return false;
	}

	private void PieceOnDropped(Piece droppedPiece, Vector2 currentPosition, Vector2 newPosition)
	{
		// reset the board so nothing is highlighted
		foreach (var (field, color) in highlightedFields)
		{
			field.Color = color;
		}
		highlightedFields.Clear();
		
		// get all possible moves
		var pieces = GetChildren().OfType<Piece>().ToArray();
		var possibleMoves = droppedPiece.Movement
			.GetMoves(pieces.Select(p => p.Movement).ToArray())
			.WithinBoard();
		
		// the move is not possible
		if (!possibleMoves.Contains(newPosition))
		{
			droppedPiece.Move(currentPosition);
			return;
		}
		
		// the move is possible
		
		// kill opponents piece if needed
		foreach (var piece in pieces)
		{
			if (piece.Movement.CurrentPosition == newPosition && piece.Movement.Player == droppedPiece.Movement.Player.GetOppositePlayer())
				piece.QueueFree();
		}
		
		// move
		droppedPiece.Move(newPosition);
		
		// keep track where the king is
		if (currentPosition == whiteKing)
		{
			whiteKing = newPosition;
		}
		else if (currentPosition == blackKing)
		{
			blackKing = newPosition;
		}
		
		// swap current player
		currentPlayer = currentPlayer.GetOppositePlayer();
		foreach (var piece in pieces)
		{
			if (piece.Movement.Player == currentPlayer)
				piece.Enable();
			else
				piece.Disable();
		}
		
		GD.Print(whiteKing);
		GD.Print(blackKing);
	}
}
