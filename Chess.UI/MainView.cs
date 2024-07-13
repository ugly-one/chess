using Chess;
using Chess.UI;
using ChessAI;
using Godot;
using System;

public partial class MainView : VBoxContainer
{
	private Button newGameButton;
	private Control menu;

	public override void _Ready()
	{
		newGameButton = GetNode<Button>("%NewGameButton");
		menu = GetNode<Control>("Menu");
		newGameButton.Pressed += OnNewGameButtonPressed;
	}

	private void OnNewGameButtonPressed()
	{
		var gameScene = GD.Load<PackedScene>("res://gameView.tscn");
		var game = gameScene.Instantiate() as GameView;
		AddChild(game);
		var isBlackAI = GetNode<CheckBox>("%BlackCheckBox").ButtonPressed;
		var allPieces = PieceFactory.CreateNewGame();
		var gameState = new Game(allPieces);
		SimpleAI blackAI = null;
		if (isBlackAI)
		{
			blackAI = new SimpleAI();
		}
		game.StartNewGame(gameState, blackAI);
		menu.Hide();
	}
}
