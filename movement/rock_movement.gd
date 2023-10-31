extends Movement

func can_move(current_position: Vector2, new_position: Vector2, player: Enums.Player) -> bool:
	if (current_position.y == new_position.y):
		return true
	if (new_position.x == current_position.x):
		return true
	return false
