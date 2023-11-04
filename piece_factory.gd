extends Node

class_name PieceFactory

static func createPieces(player: Enums.Player, backRow, frontRow) -> Array[Piece]:
	var pieces: Array[Piece]
	var king = createKing(player)
	king.chessPosition = Vector2(3,backRow)
	pieces.append(king)
	var queen = createQueen(player)
	queen.chessPosition = Vector2(4,backRow)
	pieces.append(queen)
	var rock = createRock(player)
	rock.chessPosition = Vector2(0,backRow)
	pieces.append(rock)
	var rock2 = createRock(player)
	rock2.chessPosition = Vector2(7,backRow)
	pieces.append(rock2)
	var bishop = createBishop(player)
	bishop.chessPosition = Vector2(2,backRow)
	pieces.append(bishop)
	var bishop2 = createBishop(player)
	bishop2.chessPosition = Vector2(5,backRow)
	pieces.append(bishop2)
	var knight = createKnight(player)
	knight.chessPosition = Vector2(1,backRow)
	pieces.append(knight)
	var knight2 = createKnight(player)
	knight2.chessPosition = Vector2(6,backRow)
	pieces.append(knight2)
	for i in range(0, 8):
		var pawn = createPawn(player)
		pawn.chessPosition = Vector2(i,frontRow)
		pieces.append(pawn)
	return pieces
	
static func createBishop(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/bishop_movement.tscn").instantiate()
	return createPiece(player, movement)

static func createKing(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/king_movement.tscn").instantiate()
	return createPiece(player, movement)
	
static func createQueen(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/queen_movement.tscn").instantiate()
	return createPiece(player, movement)
	
static func createRock(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/rock_movement.tscn").instantiate()
	return createPiece(player, movement)

static func createKnight(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/knight_movement.tscn").instantiate()
	return createPiece(player, movement)
	
static func createPawn(player: Enums.Player) -> Piece:
	var movement = preload("res://movement/pawn_movement.tscn").instantiate()
	return createPiece(player, movement)
	
static func createPiece(player: Enums.Player, movement: Movement):
	movement.player = player
	var piece = preload("res://piece.tscn").instantiate()
	piece.init(movement)
	return piece
	
