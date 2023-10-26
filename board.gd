extends Node2D

func _ready():
	# add black pieces
	add_piece("res://black_rook.svg", 0, 0)
	add_piece("res://black_knight.svg", 0, 1)
	add_piece("res://black_bishop.svg", 0, 2)
	add_piece("res://black_queen.svg", 0, 3)
	add_piece("res://black_king.svg", 0, 4)
	add_piece("res://black_bishop.svg", 0, 5)
	add_piece("res://black_knight.svg", 0, 6)
	add_piece("res://black_rook.svg", 0, 7)
	for n in 8:
		add_piece("res://black_pawn.svg", 1, n)
		
	# add white pieces
	add_piece("res://white_rook.svg", 7, 0)
	add_piece("res://white_knight.svg", 7, 1)
	add_piece("res://white_bishop.svg", 7, 2)
	add_piece("res://white_queen.svg", 7, 3)
	add_piece("res://white_king.svg", 7, 4)
	add_piece("res://white_bishop.svg", 7, 5)
	add_piece("res://white_knight.svg", 7, 6)
	add_piece("res://white_rook.svg", 7, 7)
	for n in 8:
		add_piece("res://white_pawn.svg", 6, n)

func add_piece(texture_path, y, x):
	var piece = load("res://piece.tscn").instantiate()
	var texture = load(texture_path)
	piece.set_texture(texture)
	piece.position.x = x * 40 + 20
	piece.position.y = y * 40 + 20
	
	piece.moved.connect(on_piece_moved)
	add_child(piece)

func get_centered_position(position: Vector2) -> Vector2:
	var column = int(position.x) / 40
	var row = int(position.y) / 40
	if (row < 0 || row > 7 || column < 0 || column > 7):
		return Vector2.ZERO
	return Vector2(column * 40 + 20, row * 40 + 20)
	
func on_piece_moved(piece, position):
	var newPosition = get_centered_position(position)
	if (newPosition == Vector2.ZERO):
		piece.position = piece.start_pos
	else:
		piece.position = newPosition
