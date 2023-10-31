extends StaticBody2D

signal dropped
signal moved

var enabled = true
var can_drag = false
var dragging = false
var startingPosition
@export var movement: Movement
@export var player: Enums.Player

func _ready():
	pass
	
func _input(event):
			
	if can_drag && event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		dragging = true
		startingPosition = position
		
	elif dragging && event is InputEventMouseButton and event.is_released and event.button_index == MOUSE_BUTTON_LEFT:
		dragging = false
		# make sure the piece is corrently placed within the board
		@warning_ignore("narrowing_conversion")
		var x: int = position.x / 40
		@warning_ignore("narrowing_conversion")
		var y: int = position.y / 40
		
		#perhaps this logic could be moved away from here
		if (x < 0 || x > 7 || y < 0 || y > 7):
			position = startingPosition
			return

		var new_position = Vector2(x, y)
		var old_x :int = startingPosition.x / 40
		var old_y :int = startingPosition.y / 40
		var current_position = Vector2(old_x, old_y)
		move2(current_position, new_position)

	elif event is InputEventMouseMotion && dragging:
		position = event.position

func move2(current_position:Vector2, new_position: Vector2):
	var can_move = movement.can_move(current_position, new_position, player)
	if (can_move == false):
		move(current_position)
	else:
		move(new_position)
		moved.emit()
	pass

func _on_mouse_shape_entered(_shape_idx):
	if dragging or (!enabled): 
		return
	can_drag = true

func _on_mouse_shape_exited(_shape_idx):
	if dragging: 
		return
	can_drag = false

func move(new_position: Vector2):
	position.x = new_position.x * 40 + 20
	position.y = new_position.y * 40 + 20
	
func enable():
	enabled = true

func disable():
	enabled = false
