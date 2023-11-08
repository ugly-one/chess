using Godot;

public partial class Piece : StaticBody2D
{
	[Signal]
	public delegate void DroppedEventHandler(Piece piece, Vector2 currentPosition, Vector2 newPosition);

	public Movement movement;
	public Player player;
	private Texture2D Texture;
	private bool canDrag;
	private bool dragging;
	private Vector2 startingPosition;
	public bool Enabled {get; set;}
	
	public void Init(Movement movement)
	{
		this.movement = movement;
		this.player = movement.Player;
		this.Texture = movement.GetTexture();
	}

	public override void _Ready()
	{
		var sprite = GetNode<Sprite2D>("Sprite2D");
		sprite.Texture = Texture;
		var position = new Vector2(movement.current_position.X * 40 + 20, movement.current_position.Y * 40 + 20);
		Position = position;
	}

	public override void _Input(InputEvent @event)
	{
		if (dragging && @event is InputEventMouseMotion motionEvent)
		{
			Position = motionEvent.Position;
		}
		
		var mouseButtonEvent = @event as InputEventMouseButton;
		if (mouseButtonEvent is null) return;

		if (canDrag  && 
		mouseButtonEvent.IsPressed() 
		&& mouseButtonEvent.ButtonIndex == MouseButton.Left)
		{
			dragging = true;
			startingPosition = Position;
		}
		else if (dragging && 
			mouseButtonEvent.IsReleased() &&
			mouseButtonEvent.ButtonIndex == MouseButton.Left
		)
		{
			dragging = false;
			canDrag = false;
			var x = (int) Position.X / 40;
			var y = (int) Position.Y / 40;
			if (x < 0 || x > 7 || y < 0 || y > 7)
			{
				Position = startingPosition;
				return;
			}
			var new_position = new Vector2(x, y);
			var old_x = (int) startingPosition.X / 40;
			var old_y = (int) startingPosition.Y / 40;
			var current_position = new Vector2(old_x, old_y);
			EmitSignal(SignalName.Dropped, this, current_position, new_position);
		}
	}

	public override void _MouseShapeEnter(int shapeIdx)
	{
		if (dragging || (!Enabled)) 
			return;
		canDrag = true;
	}

	public override void _MouseShapeExit(int shapeIdx)
	{
		if (dragging) 
			return;
		canDrag = false;
	}

	public void move(Vector2 new_position)
	{
		if (movement.current_position != new_position)
		{
			movement.Moved = true;
		}

		movement.current_position = new_position;
		var position = new Vector2(movement.current_position.X * 40 + 20, movement.current_position.Y * 40 + 20);
		Position = position;
	}


	public void disable() => Enabled = false;
	public void enable() => Enabled = true;
}
