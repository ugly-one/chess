using Godot;

namespace Chess;

public partial class PieceUI : StaticBody2D
{
	[Signal]
	public delegate void DroppedEventHandler(PieceUI piece, Vector2 currentPosition, Vector2 newPosition);
	[Signal]
	public delegate void LiftedEventHandler(PieceUI piece);

	public Piece Piece;
	private Texture2D _texture;
	private bool _isMouseOver;
	private bool _dragging;
	private Vector2 _startingPosition;
	private bool _enabled;
	
	public void Init(Piece piece, Texture2D texture)
	{
		// TODO get Vector2 instead of Piece
		Piece = piece;
		Position = new Vector2(piece.CurrentPosition.X * 40 + 20, piece.CurrentPosition.Y * 40 + 20);
		_texture = texture;
	}

	public override void _Ready()
	{
		var sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Texture = _texture;
	}

	public override void _Input(InputEvent @event)
	{
		if (!_enabled)
		{
			return;
		}
		
		if (_dragging && @event is InputEventMouseMotion motionEvent)
		{
			Position = motionEvent.Position;
		}
		
		var mouseButtonEvent = @event as InputEventMouseButton;
		if (mouseButtonEvent is null) return;

		if (_isMouseOver  && 
			mouseButtonEvent.IsPressed() 
			&& mouseButtonEvent.ButtonIndex == MouseButton.Left)
		{ 
			_dragging = true;
			_startingPosition = Position;
			GD.Print("LIFTING");
			PrintState();
			EmitSignal(SignalName.Lifted, this);
		}
		else if (_dragging && 
				 mouseButtonEvent.IsReleased() &&
				 mouseButtonEvent.ButtonIndex == MouseButton.Left
				)
		{
			_dragging = false;
			var x = (int) Position.X / 40;
			var y = (int) Position.Y / 40;
			var newPosition = new Vector2(x, y);
			var oldX = (int) _startingPosition.X / 40;
			var oldY = (int) _startingPosition.Y / 40;
			var currentPosition = new Vector2(oldX, oldY);
			
			GD.Print("DROPPING");
			PrintState();
			EmitSignal(SignalName.Dropped, this, currentPosition, newPosition);
		}
	}

	public override void _MouseShapeEnter(int shapeIdx)
	{
		if (_dragging || (!_enabled)) 
			return;
		_isMouseOver = true;
		PrintState();
	}

	private void PrintState()
	{
		GD.Print($"{this.Piece.GetType()} {this.Piece.Player} isMouseOver: {_isMouseOver}");
		GD.Print($"{this.Piece.GetType()} {this.Piece.Player} dragging: {_dragging}");
		GD.Print($"{this.Piece.GetType()} {this.Piece.Player} enabled: {_enabled}");
	}

	public override void _MouseShapeExit(int shapeIdx)
	{
		if (_dragging) 
			return;
		_isMouseOver = false;
		PrintState();
	}

	public void Move(Vector2 newPosition)
	{
		var position = new Vector2(newPosition.X * 40 + 20, newPosition.Y * 40 + 20);
		Position = position;
	}

	public void Disable() => _enabled = false;
	public void Enable() => _enabled = true;
}
