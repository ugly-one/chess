extends Movement
class_name PawnMovement

var player: Enums.Player

func can_move(new_position: Vector2) -> bool:
	if (current_position.y == new_position.y):
		return false
	if (new_position.x != current_position.x):
		return false

	#TODO add support for taking opponents pieces
	if (player == Enums.Player.WHITE):
		if (new_position.y - 1 == current_position.y or (!moved and new_position.y - 2 == current_position.y)):
			return true
		else: 
			return false
	else:
		if (new_position.y + 1 == current_position.y or (!moved and new_position.y + 2 == current_position.y)):
			return true
		else:
			return false

func get_possible_moves() -> Array[Vector2]:
	var moves : Array[Vector2] = []
	if (player == Enums.Player.WHITE):
		moves.append(current_position + (Vector2(-1, 1)))
		moves.append(current_position + (Vector2(1, 1)))
		moves.append(current_position + (Vector2(0, 1)))
		if (!moved):
			moves.append(current_position + (Vector2(0, 2)))
	else:
		moves.append(current_position + (Vector2(-1, -1)))
		moves.append(current_position + (Vector2(1, -1)))
		moves.append(current_position + (Vector2(0, -1)))
		if (!moved):
			moves.append(current_position + (Vector2(0, -2)))
	return moves
	
func get_texture():
	return _get_texture(player, "pawn")
