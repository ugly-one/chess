extends Node2D

@export var player: Enums.Player = Enums.Player.WHITE
@export var movement: PawnMovement

func _ready():
	var texture: CompressedTexture2D = preload("res://assets/black_pawn.svg")
	$piece.texture_a = texture

func _on_piece_dropped(current_position:Vector2, new_position: Vector2):
	var can_move = movement.can_move(current_position, new_position, player)
	if (can_move == false):
		$piece.move(current_position)
	pass # Replace with function body.
