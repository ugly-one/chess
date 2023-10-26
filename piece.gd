extends Sprite2D

signal moved

var isMoving = false
var fieldSize = 40
var start_pos

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

	if event is InputEventMouseMotion && isMoving:
		var vector = event.position - start_pos
		position = start_pos + vector

func set_chess_position(x, y):
	position.x = x * fieldSize + fieldSize/2
	position.y = y * fieldSize + fieldSize/2

func get_centered_position(position: Vector2) -> Vector2:
	var column = int(position.x) / fieldSize
	var row = int(position.y) / fieldSize
	if (row < 0 || row > 7 || column < 0 || column > 7):
		return Vector2.ZERO
		
	# check if this is a valid move
	var previousColumn = int(start_pos.x) / fieldSize
	var previousRow = int(start_pos.y) / fieldSize
	
	# move as white pawn
	if (column != previousColumn):
		return Vector2.ZERO
	if (previousRow - 1 != row):
		return Vector2.ZERO
	
	return Vector2(column * fieldSize + fieldSize/2, row * fieldSize + fieldSize/2)
	
func drop():
	var newPosition = get_centered_position(position)
	if (newPosition == Vector2.ZERO):
		position = start_pos
	else:
		position = newPosition
