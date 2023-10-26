extends Node2D

# Called when the node enters the scene tree for the first time.
func _ready():
	var piece = load("res://piece.tscn").instantiate()
	piece.position.x = 4 * 40 + 20
	piece.position.y = 0 * 40 + 20
	piece.moved.connect(on_piece_moved)
	
	add_child(piece)
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func place(piece, position):
	var newPosition = get_centered_position(position)
	if (newPosition == Vector2.ZERO):
		piece.position = Vector2(20, 20)
	else:
		piece.position = newPosition

func get_centered_position(position: Vector2) -> Vector2:
	var column = int(position.x) / 40
	var row = int(position.y) / 40
	if (row < 0 || row > 7 || column < 0 || column > 7):
		return Vector2.ZERO
	return Vector2(column * 40 + 20, row * 40 + 20)
	
func on_piece_moved(piece, position):
	place(piece, position)
