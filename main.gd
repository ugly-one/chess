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
		piece.connect("dropped", _on_piece_dropped)

func _on_piece_dropped(piece: Piece, current_position: Vector2, new_position: Vector2):
	piece.move(new_position)
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
