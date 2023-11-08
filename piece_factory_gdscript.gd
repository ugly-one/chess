extends Node

class_name PieceFactory_gdscript

static func createPieces(player: Enums.Player, backRow, frontRow):
	var pieces: Array
	var king = createKing(player, Vector2(3,backRow))
	pieces.append(king)
	var queen = createQueen(player, Vector2(4,backRow))
	pieces.append(queen)
	var bishop = createBishop(player, Vector2(2,backRow))
	pieces.append(bishop)
	var bishop2 = createBishop(player, Vector2(5,backRow))
	pieces.append(bishop2)
	var rock = createRock(player, Vector2(0,backRow))
	pieces.append(rock)
	var rock2 = createRock(player, Vector2(7,backRow))
	pieces.append(rock2)
	var knight = createKnight(player, Vector2(1,backRow))
	pieces.append(knight)
	var knight2 = createKnight(player, Vector2(6,backRow))
	pieces.append(knight2)
	for i in range(0, 8):
		var pawn = createPawn(player, Vector2(i,frontRow))
		pieces.append(pawn)

	return pieces
	
static func createBishop(player: Enums.Player, pos):
	var script = load("res://movement/BishopMovement.cs")
	return createPiece(player, script, pos)

static func createPiece(player, script, pos):
	var movement = script.new()
	movement.Player = player
	movement.current_position = pos
	var piece_scene = preload("res://piece_csharp.tscn")
	var piece = piece_scene.instantiate()
	piece.Init(movement)
	return piece

static func createKing(player: Enums.Player, pos):
	var script = load("res://movement/KingMovement.cs")
	return createPiece(player, script, pos)
	
static func createQueen(player: Enums.Player, pos):
	var script = load("res://movement/QueenMovement.cs")
	return createPiece(player, script, pos)
	
static func createRock(player: Enums.Player, pos):
	var script = load("res://movement/RockMovement.cs")
	return createPiece(player, script, pos)

static func createKnight(player: Enums.Player, pos):
	var script = load("res://movement/KnightMovement.cs")
	return createPiece(player, script, pos)
	
static func createPawn(player: Enums.Player, pos):
	var script = load("res://movement/PawnMovement.cs")
	return createPiece(player, script, pos)	
