extends Movement

func can_move(current_position: Vector2, new_position: Vector2, player: Enums.Player) -> bool:
	var move_vector = abs(new_position-current_position)
	if (move_vector.x == move_vector.y):
		return true
	if (current_position.y == new_position.y):
		return true
	if (new_position.x == current_position.x):
		return true
	return false
