extends Sprite2D

signal moved

var isMoving = false

# I think I can simplify the move now - we do not need those 2 properties perhaps
var start_pos
var start_event_pos

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

func _input(event):
	
	if (event is InputEventMouseButton 
		and event.is_released() 
		and event.button_index == MOUSE_BUTTON_LEFT
		and isMoving):
			isMoving = false
			moved.emit(self, position)
			
	if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
		if get_rect().has_point(to_local(event.position)):
			isMoving = true
			start_pos = position
			start_event_pos = event.position

	if event is InputEventMouseMotion && isMoving:
		var vector = event.position - start_event_pos
		position = start_pos + vector
