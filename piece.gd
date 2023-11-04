extends StaticBody2D
class_name Piece

signal dropped(piece: Piece, current_position: Vector2, new_position: Vector2)

var enabled = true
var can_drag = false
var dragging = false
var startingPosition
var texture
var player # this is needed to be exposed so the board know which player owns this piece
@export var movement: Movement
@onready var sprite_2d = %Sprite2D

func _ready():
	sprite_2d.texture = texture
	position.x = movement.current_position.x * 40 + 20
	position.y = movement.current_position.y * 40 + 20
	pass
	
func init(movement: Movement):
	self.movement = movement
	self.player = movement.player
	self.texture = movement.get_texture()
	
func _input(event):
	if can_drag && event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		dragging = true
		startingPosition = position
		
	elif dragging && event is InputEventMouseButton and event.is_released and event.button_index == MOUSE_BUTTON_LEFT:
		dragging = false
		can_drag = false
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
		drop(current_position, new_position)

	elif event is InputEventMouseMotion && dragging:
		position = event.position

func drop(current_position:Vector2, new_position: Vector2):
	if (movement.can_move(new_position)):
		dropped.emit(self, current_position, new_position)
	else:
		move(current_position)
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
	if (movement.current_position != new_position):
		movement.moved = true
		
	movement.current_position = new_position
	position.x = new_position.x * 40 + 20
	position.y = new_position.y * 40 + 20
	
func enable():
	enabled = true

func disable():
	enabled = false
