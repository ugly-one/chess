extends Node2D

var currentPlayer = Enums.Player.WHITE

func _ready():
	var player = Enums.Player.WHITE
	var white_pieces = PieceFactory.createPieces(Enums.Player.WHITE, 0, 1)
	var black_pieces = PieceFactory.createPieces(Enums.Player.BLACK, 7, 6)
	for piece in white_pieces:
		add_child(piece)
		piece.connect("dropped", _on_piece_dropped)
	for piece in black_pieces:
		add_child(piece)
		piece.disable()
		piece.connect("dropped", _on_piece_dropped)

func _on_piece_dropped(piece: Piece, current_position: Vector2, new_position: Vector2):
	var children = get_children()
	# disable dropping pieces on top of your own pieces
	var illegalMove = false
	for child in children:
		if ("player" in child):
			if (child.player == piece.player and new_position == child.chessPosition):
				piece.move(current_position)
				illegalMove = true
	if illegalMove:
		return
	
	#accept the move
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
