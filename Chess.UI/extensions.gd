extends Node

enum ChessColor{
	WHITE,
	BLACK
}

func getTexture(color: ChessColor, piece: String):
	var colorAsString: String = "white"
	if (color == ChessColor.BLACK):
		colorAsString = "black"
	pass
	
	var path: String = "res://assets/" + colorAsString + "_" + piece + ".svg"
	
	print(path)
	var texture: Texture2D = load(path)
	return texture
