extends Node2D

var currentPlayer = Enums.Player.WHITE

func _on_piece_moved():
	if currentPlayer == Enums.Player.WHITE:
		currentPlayer = Enums.Player.BLACK
	else:
		currentPlayer = Enums.Player.WHITE
		
	var children = get_children()
	for child in children:
		if ("player" in child):
			if (child.player == currentPlayer):
				child.enable()
			else:
				child.disable()


func _on_pawn_moved():
	_on_piece_moved()

func _on_rook_moved():
	_on_piece_moved()
