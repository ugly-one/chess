extends Movement
class_name BishopMovement

var player: Enums.Player

func can_move(new_position: Vector2) -> bool:
	var move_vector = abs(new_position-current_position)
	if (move_vector.x == move_vector.y):
		return true
	return false

func get_texture():
	return _get_texture(player, "bishop")
