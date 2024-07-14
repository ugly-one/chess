using Chess;
using Chess.UI;
using ChessAI;
using Godot;
using System;

public partial class MainView : VBoxContainer
{
	private Button newGameButton;
	private Button goToMainMenuButton;
	private Control newGameMenu;
	private Control currentGameMenu;

	public override void _Ready()
	{
		newGameButton = GetNode<Button>("%NewGameButton");
		goToMainMenuButton = GetNode<Button>("%MainMenuButton");
		newGameMenu = GetNode<Control>("NewGameMenu");
		currentGameMenu = GetNode<Control>("CurrentGameMenu");
		currentGameMenu.Hide();
		newGameButton.Pressed += OnNewGameButtonPressed;
		goToMainMenuButton.Pressed += OnGoToMainMenuButtonPressed;
	}

	private void OnNewGameButtonPressed()
	{
		var gameScene = GD.Load<PackedScene>("res://gameView.tscn");
		var game = gameScene.Instantiate() as GameView;
		AddChild(game);
		var isBlackAI = GetNode<CheckBox>("%BlackCheckBox").ButtonPressed;
		var isWhiteAI = GetNode<CheckBox>("%WhiteCheckBox").ButtonPressed;
		var allPieces = PieceFactory.CreateNewGame();
		var gameState = new Game(allPieces);
		PlayerHost blackAI = null;
		if (isBlackAI)
		{
			blackAI = new PlayerHost(new RandomMoveAI(Color.BLACK));
		}
		PlayerHost whiteAI = null;
		if (isWhiteAI)
		{
			whiteAI = new PlayerHost(new GiveMeCheckAI(Color.WHITE));
		}
		game.StartNewGame(gameState, black: blackAI, white: whiteAI);
		game.SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
		newGameMenu.Hide();
		currentGameMenu.Show();
	}

	private void OnGoToMainMenuButtonPressed()
	{
		var game = GetNode<GameView>("GameView");
		game.QueueFree();
		currentGameMenu.Hide();
		newGameMenu.Show();
	}
}
