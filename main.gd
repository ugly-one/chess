extends Node2D

var currentPlayer = Enums.Player.WHITE

func _ready():
	var player = Enums.Player.WHITE
	var pieceFactoryScript = preload("res://PieceFactory.cs").new()
	var white_pieces = pieceFactoryScript.CreatePieces(Enums.Player.WHITE, 0, 1)
	var black_pieces = pieceFactoryScript.CreatePieces(Enums.Player.BLACK, 7, 6)
	for piece in white_pieces:
		add_child(piece)
		piece.Enabled = true
		piece.connect("Dropped", _on_piece_dropped)
	for piece in black_pieces:
		add_child(piece)
		piece.disable()
		piece.connect("Dropped", _on_piece_dropped)
		
func _on_piece_dropped(piece, current_position: Vector2, new_position: Vector2):

	if (!piece.movement.can_move(new_position)):
		piece.move(current_position)
		return
	
	var children = get_children()
	# disable dropping pieces on top of your own pieces
	var illegalMove = false
	
	for child in children:
		if ("player" in child):
			if("player" in piece):
				if (child.player == piece.player and new_position == child.movement.current_position):
					illegalMove = true

	if illegalMove:
		piece.move(current_position)
		return
		
	# disable dropping pieces if their path to the destination is not clear
	var path: Array[Vector2]
	path = get_fields_on_path(current_position, new_position)
	for child in children:
		if ("player" in child):
			if (path.has(child.movement.current_position)):
				illegalMove = true
				
	if illegalMove:
		piece.move(current_position)
		return
	
	# TODO support en passant
	# TODO make moves, that expose current's player king to be under attack, illegal
	
	#accept the move

	for child in children:
		if ("player" in child):
			if (child.movement.current_position == new_position):
				if (child.player == getOpositePlayer(currentPlayer)):
					child.queue_free()
					print("KILL!")
	
	piece.move(new_position)
	
	# switch the current player
	if currentPlayer == Enums.Player.WHITE:
		currentPlayer = Enums.Player.BLACK
	else:
		currentPlayer = Enums.Player.WHITE
	
	# disable non-active player
	for child in children:
		if ("player" in child):
			if (child.player == currentPlayer):
				child.enable()
			else:
				child.disable()

func getOpositePlayer(player):
	if (player == Enums.Player.WHITE):
		return Enums.Player.BLACK
	else:
		return Enums.Player.WHITE
		
func get_fields_on_path(start: Vector2, end: Vector2) -> Array[Vector2]:
	var path: Array[Vector2]
	if (abs( start - end ) == Vector2(1,0) || abs(start - end) == Vector2(0,1)):
		# if we're moving only one field - no path to check
		return path
		
	var diff = end - start
	
	if (diff.x == 0 || diff.y == 0):
		var fieldsInBetween = diff.length()
		var field = start
		for i in fieldsInBetween - 1:
			field = Vector2(field + diff.normalized())
			path.append(field)
		return path
	
	if (abs(diff.x) == abs(diff.y)):
		var xDirection = sign(diff.x)
		var yDirection = sign(diff.y)
		
		var fieldsInBetween = abs(start.x - end.x)
		var field = start
		for i in fieldsInBetween - 1:
			field = Vector2(field + Vector2(xDirection,yDirection))
			path.append(field)
		return path
	
	return path
