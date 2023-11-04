extends Node2D

var currentPlayer = Enums.Player.WHITE

func _ready():
	var player = Enums.Player.WHITE
	createPieces(Enums.Player.WHITE, 0, 1)
	createPieces(Enums.Player.BLACK, 7, 6)

func createPieces(player: Enums.Player, backRow, frontRow):
	var king = createKing(player)
	king.move(Vector2(3,backRow))
	add_child(king)
	var queen = createQueen(player)
	queen.move(Vector2(4,backRow))
	add_child(queen)
	var rock = createRock(player)
	rock.move(Vector2(0,backRow))
	add_child(rock)
	var rock2 = createRock(player)
	rock2.move(Vector2(7,backRow))
	add_child(rock2)
	var bishop = createBishop(player)
	bishop.move(Vector2(2,backRow))
	add_child(bishop)
	var bishop2 = createBishop(player)
	bishop2.move(Vector2(5,backRow))
	add_child(bishop2)
	var knight = createKnight(player)
	knight.move(Vector2(1,backRow))
	add_child(knight)
	var knight2 = createKnight(player)
	knight2.move(Vector2(6,backRow))
	add_child(knight2)
	for i in range(0, 8):
		var pawn = createPawn(player)
		pawn.move(Vector2(i,frontRow))
		add_child(pawn)
	
func createBishop(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/bishop_movement.tscn").instantiate()
	return createPiece(player, movement)

func createKing(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/king_movement.tscn").instantiate()
	return createPiece(player, movement)
	
func createQueen(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/queen_movement.tscn").instantiate()
	return createPiece(player, movement)
	
func createRock(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/rock_movement.tscn").instantiate()
	return createPiece(player, movement)

func createKnight(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/knight_movement.tscn").instantiate()
	return createPiece(player, movement)
	
func createPawn(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/pawn_movement.tscn").instantiate()
	return createPiece(player, movement)
	
func createPiece(player: Enums.Player, movement: Movement):
	movement.player = player
	var piece = preload("res://piece.tscn").instantiate()
	piece.init(movement)
	return piece
	
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
