using System;
using Godot;

namespace Chess;

public partial class PieceUI : StaticBody2D
{
	[Signal]
	public delegate void DroppedEventHandler(PieceUI piece, Vector2 currentPosition, Vector2 newPosition);
	[Signal]
	public delegate void LiftedEventHandler(PieceUI piece, Vector2 currentPosition);

	public Color Color;
	public Vector2 ChessPosition;
	private Texture2D _texture;
	private bool _isMouseOver;
	private bool _dragging;
	private Vector2 _startingPosition;
	private Vector2 _mouseClickPosition;
	private bool _enabled;
	
	public void Init(Vector2 position, Color color, Texture2D texture)
	{
		ChessPosition = position;
		Position = new Vector2(position.X * 40 + 20, position.Y * 40 + 20);
		_texture = texture;
		Color = color;
	}
	
	public void Move(Vector2 newPosition)
	{
		ChessPosition = newPosition;
		Position = new Vector2(newPosition.X * 40 + 20, newPosition.Y * 40 + 20);
	}

	public void SnapToPositionWithoutChangingChessPosition(Vector2 newPosition)
	{
		Position = new Vector2(newPosition.X * 40 + 20, newPosition.Y * 40 + 20);
	}

	public void ChangeTexture(Texture2D texture)
	{
		_texture = texture;
		var sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Texture = _texture;
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
			Position = _startingPosition + motionEvent.Position - _mouseClickPosition;
		}
		
		var mouseButtonEvent = @event as InputEventMouseButton;
		if (mouseButtonEvent is null) return;

		if (_isMouseOver  && 
			mouseButtonEvent.IsPressed() 
			&& mouseButtonEvent.ButtonIndex == MouseButton.Left)
		{ 
			_dragging = true;
			_startingPosition = Position;
			_mouseClickPosition = mouseButtonEvent.Position;
			GD.Print(_mouseClickPosition);
			var x = (int) Position.X / 40;
			var y = (int) Position.Y / 40;
			var position = new Vector2(x, y);
			ZIndex = 1;
			EmitSignal(SignalName.Lifted, this, position);
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
			ZIndex = 0;
			EmitSignal(SignalName.Dropped, this, currentPosition, newPosition);
		}
	}

	public override void _MouseShapeEnter(int shapeIdx)
	{
		if (_dragging || (!_enabled)) 
			return;
		_isMouseOver = true;
	}

	public override void _MouseShapeExit(int shapeIdx)
	{
		if (_dragging) 
			return;
		_isMouseOver = false;
	}

	public void CancelMove()
	{
		Position = _startingPosition;
	}

	public void Disable() => _enabled = false;
	public void Enable() => _enabled = true;
}
