extends Sprite2D

signal moved

var isMoving = false

# I think I can simplify the move now - we do not need those 2 properties perhaps
var start_pos
var start_event_pos

func _input(event):
	
	if (event is InputEventMouseButton 
		and event.is_released() 
		and event.button_index == MOUSE_BUTTON_LEFT
		and isMoving):
			isMoving = false
			drop()
			
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		if get_rect().has_point(to_local(event.position)):
			isMoving = true
			start_pos = position
			start_event_pos = event.position

	if event is InputEventMouseMotion && isMoving:
		var vector = event.position - start_event_pos
		position = start_pos + vector

func set_chess_position(x, y):
	position.x = x * 40 + 20
	position.y = y * 40 + 20

func get_centered_position(position: Vector2) -> Vector2:
	var column = int(position.x) / 40
	var row = int(position.y) / 40
	if (row < 0 || row > 7 || column < 0 || column > 7):
		return Vector2.ZERO
	return Vector2(column * 40 + 20, row * 40 + 20)
	
func drop():
	var newPosition = get_centered_position(position)
	if (newPosition == Vector2.ZERO):
		position = start_pos
	else:
		position = newPosition
