extends Node

class_name Movement

func can_move(_current_position: Vector2, _new_position: Vector2) -> bool:
	return false

func _get_texture(player: Enums.Player, piece):
	var color = "white" if player == Enums.Player.WHITE else "black"
	var image = Image.load_from_file("res://assets/" + color + "_" + piece + ".svg")
	return ImageTexture.create_from_image(image)
