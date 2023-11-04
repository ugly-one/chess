extends Movement
class_name KnightMovement

var player: Enums.Player

func can_move(current_position: Vector2, new_position: Vector2) -> bool:
	if abs(new_position.x - current_position.x) == 1 and abs(new_position.y - current_position.y) == 2:
		return true
	if abs(new_position.x - current_position.x) == 2 and abs(new_position.y - current_position.y) == 1:
		return true
	return false

func get_texture():
	return _get_texture(player, "knight")