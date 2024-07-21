Chess. Yeap, groundbreaking, never done before.

The main 2 parts are:
- Engine (`Chess` project)
- UI that shows the game/board (`Chess.UI`)

The engine is written in C#. 
UI is also written in C# but it uses Godot. I think I would like to re-write it in GDscript. 

UI supports 
- playing manually - where you have to make each move.
- playing against a simple AI. 
- having 2 AIs play against each other.

In order to run the UI project, one needs Godot to be installed. This is subotimal, and hopefully will be improved in the future.

Also, I'm thinking to split the repo into 2 separates repositories:
- one only for the engine
- one only for the UI

Thanks to that it will be easier to track what needs to be done for each part. 

# TODOs

- add perft tests https://www.chessprogramming.org/Perft_Results
- be able to setup a board manually - needed to debug specific positions
- stop/resume AIs games
- show/hide analyse panel (debugging)
- present history of moves in text format (debugging)
- load a DLL with an AI - test if it's possible to debug such bot
- prevent setting up invalid boards

# NICE TO HAVE

- exporting/importing games - it would be cool to share games with others
- create `ChessField` which will take care of its own color (highlighting it and reseting the color) and will host pieces
so we don't have to take care of snapping the pieces in PieceUI
- make UI nicer. example: captured pieces should be smaller and in 2/3 rows. 45px is spread across code base (fx: capturedPieces has it or PieceUI).
Try to change the size of a piece and see stuff breaking
- promotion logic needs some love - not sure about this anymore :D
