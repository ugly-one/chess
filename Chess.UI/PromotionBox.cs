using Godot;
using Chess;

public partial class PromotionBox : VBoxContainer
{
	[Signal]
	public delegate void PieceForPromotionSelectedEventHandler(PieceUI pieceToUpgrade, Vector2 newPosition, string newPieceType);
	private TextureButton bishop;
	private TextureButton knight;
	private TextureButton rock;
	private TextureButton queen;
	
	private PieceUI pieceToMove;
	private Vector2 newPosition;

	public override void _Ready()
	{
		bishop = GetNode<TextureButton>("%bishop");
		bishop.Pressed += () => OnButtonPressed(PieceType.Bishop);
		knight = GetNode<TextureButton>("%knight");
		knight.Pressed += () => OnButtonPressed(PieceType.Knight);
		rock = GetNode<TextureButton>("%rock");
		rock.Pressed += () => OnButtonPressed(PieceType.Rock);
		queen = GetNode<TextureButton>("%queen");
		queen.Pressed += () => OnButtonPressed(PieceType.Queen);
	}

	private void OnButtonPressed(Chess.PieceType type)
	{
		// bleh... I can't emit enum so we have to emit for instance a string?!?!
		EmitSignal(SignalName.PieceForPromotionSelected, pieceToMove, newPosition, type.ToString());
	}

	public void Bla(PieceUI pieceToMove, Vector2 newPosition)
	{
		this.pieceToMove = pieceToMove;
		this.newPosition = newPosition;
		var color = pieceToMove.Color;
		bishop.TextureNormal = color.GetTexture("bishop");
		knight.TextureNormal = color.GetTexture("knight");
		rock.TextureNormal = color.GetTexture("rock");
		queen.TextureNormal = color.GetTexture("queen");
		Show();
	}
}
