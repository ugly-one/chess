using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Chess.UI;

public partial class AnalysePanel : VBoxContainer
{
	private Node? _analysisBoard;
	private Button? _nextMoveButton;
	private Button? _previousMoveButton;
	private List<Board> _boards;
	private int _currentBoardIndex = 0;
	public override void _Ready()
	{
		_analysisBoard = GetNode("%analysis_board");
		_nextMoveButton = GetNode<Button>("%nextMoveButton");
		_nextMoveButton.Pressed += OnNextMoveButtonPressed;
		_previousMoveButton = GetNode<Button>("%previousMoveButton");
		_previousMoveButton.Pressed += OnPreviousMoveButtonPressed;
		_boards = new List<Board>();
	}
	
	
	public void Display(Board gameBoard)
	{
		_boards.Add(gameBoard);
		_currentBoardIndex = _boards.Count - 1;
		
		DisplayPrivate(gameBoard);
	}

	public void Reset()
	{
		_currentBoardIndex = 0;
		_boards = new List<Board>();
	}

	private void OnPreviousMoveButtonPressed()
	{
		if (_currentBoardIndex == 0)
		{
			return;
		}
		_currentBoardIndex -= 1;
		DisplayPrivate(_boards[_currentBoardIndex]);
	}

	private void OnNextMoveButtonPressed()
	{
		if (_currentBoardIndex == _boards.Count - 1)
		{
			return;
		}
		_currentBoardIndex += 1;
		DisplayPrivate(_boards[_currentBoardIndex]);
	}


	private void DisplayPrivate(Board gameBoard)
	{
		var analysisPieces = _analysisBoard.GetChildren().OfType<PieceUI>();
		foreach (var analysePiece in analysisPieces)
		{
			analysePiece.QueueFree();
		}

		var analysis_piecesUi =
			gameBoard.GetPieces().Select(p => PieceFactory.CreatePiece(p.Position, p.Color, p.GetTexture()));
		foreach (var pieceUi in analysis_piecesUi)
		{
			_analysisBoard.AddChild(pieceUi);
		}
	}
}

