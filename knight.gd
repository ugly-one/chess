extends Node2D

@export var player: Enums.Player = Enums.Player.WHITE
signal moved

func _ready():
	var black_texture: CompressedTexture2D = preload("res://assets/black_knight.svg")
	var white_texture: CompressedTexture2D = preload("res://assets/white_knight.svg")
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
	if abs(new_position.x - current_position.x) == 1 and abs(new_position.y - current_position.y) == 2:
		return true
	if abs(new_position.x - current_position.x) == 2 and abs(new_position.y - current_position.y) == 1:
		return true
	return false

func enable():
	$piece.enabled = true

func disable():
	$piece.enabled = false
