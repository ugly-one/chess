extends Node2D

@export var player: Enums.Player = Enums.Player.WHITE

func _ready():
	var black_texture: CompressedTexture2D = preload("res://assets/black_pawn.svg")
	var white_texture: CompressedTexture2D = preload("res://assets/white_pawn.svg")
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
	pass

func can_move(current_position: Vector2, new_position: Vector2) -> bool:
	if (current_position.y == new_position.y):
		return false
	if (new_position.x != current_position.x):
		return false
	if (player == Enums.Player.WHITE):
		if (new_position.y + 1 == current_position.y):
			return true
		else: 
			return false
	else :
		if (new_position.y - 1 == current_position.y):
			return true
		else:
			return false
