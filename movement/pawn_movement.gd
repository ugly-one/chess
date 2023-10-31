extends Movement

func can_move(current_position: Vector2, new_position: Vector2, player: Enums.Player) -> bool:
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
