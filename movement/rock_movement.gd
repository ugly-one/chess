extends Movement
class_name RockMovement

var player: Enums.Player

func can_move(current_position: Vector2, new_position: Vector2) -> bool:
	if (current_position.y == new_position.y):
		return true
	if (new_position.x == current_position.x):
		return true
	return false

func get_texture():
	return _get_texture(player, "rock")
