extends Node2D

@export var player: Enums.Player = Enums.Player.WHITE
signal moved

func _ready():
	var black_texture: CompressedTexture2D = preload("res://assets/black_bishop.svg")
	var white_texture: CompressedTexture2D = preload("res://assets/white_bishop.svg")
	if (player == Enums.Player.WHITE):
		$piece.set_texture(white_texture)
	else:
		$piece.set_texture(black_texture)

func _on_piece_dropped(current_position:Vector2, new_position: Vector2):
	var can_move = can_move(current_position, new_position)
	if (can_move == false):
		$piece.move(current_position)
	else:
		$piece.move(new_position)
		moved.emit()
	pass

func can_move(current_position: Vector2, new_position: Vector2) -> bool:
	var move_vector = abs(new_position-current_position)
	if (move_vector.x == move_vector.y):
		return true
	return false

func enable():
	$piece.enabled = true

func disable():
	$piece.enabled = false
