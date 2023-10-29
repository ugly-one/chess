extends Node2D

var startingPosition

func _on_piece_dropped(piece: Node2D):
	# make sure the piece is corrently placed within the board
	var x: int = piece.position.x / 40
	var y: int = piece.position.y / 40
	if (x < 0 || x > 7 || y < 0 || y > 7):
		piece.position = startingPosition
		return
	piece.position.x = x * 40 + 20
	piece.position.y = y * 40 + 20

func _on_piece_lifted(piece: Node2D):
	startingPosition = piece.position
