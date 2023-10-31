extends Movement

func can_move(current_position: Vector2, new_position: Vector2, player: Enums.Player) -> bool:
	if abs(new_position.x - current_position.x) == 1 and abs(new_position.y - current_position.y) == 2:
		return true
	if abs(new_position.x - current_position.x) == 2 and abs(new_position.y - current_position.y) == 1:
		return true
	return false
