using System.Linq;
using Godot;

namespace Bla;

public partial class ChessEngine : Node2D
{
	private Player currentPlayer = Player.WHITE;

	public override void _Ready()
	{
		var pieceFactory = new PieceFactory();
		var whitePieces = pieceFactory.CreatePieces(Player.WHITE, 7, 6);
		var blackPieces = pieceFactory.CreatePieces(Player.BLACK, 0, 1);

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

		var possibleMoves = piece.Movement.GetMoves(pieces, piece.Movement.CurrentPosition);
		foreach (var possibleMove in possibleMoves)
		{
			GD.Print(possibleMove);
		}
	}

	private void PieceOnDropped(Piece droppedPiece, Vector2 currentPosition, Vector2 newPosition)
	{
		
		if (!droppedPiece.Movement.CanMove(newPosition))
		{
			droppedPiece.Move(currentPosition);
			return;
		}

		var pieces = GetChildren().OfType<Piece>().ToArray();
		
		// disable dropping pieces on top of your own pieces
		foreach (var piece in pieces)
		{
			if (newPosition == piece.Movement.CurrentPosition && piece.Player == droppedPiece.Player)
			{
				droppedPiece.Move(currentPosition);
				return;
			}
		}
		
		// disable dropping pieces if their path to the destination is not clear
		var path = currentPosition.GetFieldsOnPathTo(newPosition);
		foreach (var piece in pieces)
		{
			if (path.Contains(piece.Movement.CurrentPosition))
			{
				droppedPiece.Move(currentPosition);
				return;
			}
		}
		
		// kill opponents piece if needed
		foreach (var piece in pieces)
		{
			if (piece.Movement.CurrentPosition == newPosition && piece.Player == GetOppositePlayer(droppedPiece.Player))
				piece.QueueFree();
		}
		
		droppedPiece.Move(newPosition);
		currentPlayer = GetOppositePlayer(currentPlayer);

		foreach (var piece in pieces)
		{
			if (piece.Player == currentPlayer)
				piece.Enable();
			else
				piece.Disable();
		}
	}

	private Player GetOppositePlayer(Player player)
	{
		return player == Player.BLACK ? Player.WHITE : Player.BLACK;
	}
}
