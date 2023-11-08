using Godot;

namespace Bla;

public partial class Piece : StaticBody2D
{
	[Signal]
	public delegate void DroppedEventHandler(Piece piece, Vector2 currentPosition, Vector2 newPosition);
	[Signal]
	public delegate void LiftedEventHandler(Piece piece);

	public movement.Movement Movement;

	private Texture2D _texture;
	private bool _canDrag;
	private bool _dragging;
	private Vector2 _startingPosition;
	private bool _enabled;
	
	public void Init(movement.Movement movement)
	{
		Movement = movement;
		_texture = movement.GetTexture();
	}

	public override void _Ready()
	{
		var sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Texture = _texture;
		Position = new Vector2(Movement.CurrentPosition.X * 40 + 20, Movement.CurrentPosition.Y * 40 + 20);
	}

	public override void _Input(InputEvent @event)
	{
		if (_dragging && @event is InputEventMouseMotion motionEvent)
		{
			Position = motionEvent.Position;
		}
		
		var mouseButtonEvent = @event as InputEventMouseButton;
		if (mouseButtonEvent is null) return;

		if (_canDrag  && 
			mouseButtonEvent.IsPressed() 
			&& mouseButtonEvent.ButtonIndex == MouseButton.Left)
		{
			_dragging = true;
			_startingPosition = Position;
			EmitSignal(Piece.SignalName.Lifted, this);
		}
		else if (_dragging && 
				 mouseButtonEvent.IsReleased() &&
				 mouseButtonEvent.ButtonIndex == MouseButton.Left
				)
		{
			_dragging = false;
			_canDrag = false;
			var x = (int) Position.X / 40;
			var y = (int) Position.Y / 40;
			var newPosition = new Vector2(x, y);
			var oldX = (int) _startingPosition.X / 40;
			var oldY = (int) _startingPosition.Y / 40;
			var currentPosition = new Vector2(oldX, oldY);
			EmitSignal(Piece.SignalName.Dropped, this, currentPosition, newPosition);
		}
	}

	public override void _MouseShapeEnter(int shapeIdx)
	{
		if (_dragging || (!_enabled)) 
			return;
		_canDrag = true;
	}

	public override void _MouseShapeExit(int shapeIdx)
	{
		if (_dragging) 
			return;
		_canDrag = false;
	}

	public void Move(Vector2 newPosition)
	{
		if (Movement.CurrentPosition != newPosition)
		{
			Movement.Moved = true;
		}

		Movement.CurrentPosition = newPosition;
		var position = new Vector2(Movement.CurrentPosition.X * 40 + 20, Movement.CurrentPosition.Y * 40 + 20);
		Position = position;
	}


	public void Disable() => _enabled = false;
	public void Enable() => _enabled = true;
}
