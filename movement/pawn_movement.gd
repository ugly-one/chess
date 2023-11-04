extends Movement
class_name PawnMovement

var player: Enums.Player

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

func get_texture():
	return _get_texture(player, "pawn")
