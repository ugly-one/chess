extends Node2D

var currentPlayer = Enums.Player.WHITE

func _ready():
	var pieceScene = preload("res://piece.tscn")
	var bishopMovement = preload("res://movement/bishop_movement.tscn").instantiate()
	var knightMovement = preload("res://movement/knight_movement.tscn").instantiate()
	var kingMovement = preload("res://movement/king_movement.tscn").instantiate()	
	kingMovement.player = Enums.Player.WHITE
	knightMovement.player = Enums.Player.BLACK
	bishopMovement.player = Enums.Player.BLACK
	
	var bishop = pieceScene.instantiate()
	bishop.init(bishopMovement)
	add_child(bishop)
	var knight = pieceScene.instantiate()
	knight.init(knightMovement)
	add_child(knight)
	var king = pieceScene.instantiate()
	king.init(kingMovement)
	add_child(king)

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
