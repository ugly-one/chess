using Chess.UI;
using Godot;
using System;

public partial class MainView : Node2D
{
	private Button _newGameButton;

	public override void _Ready()
	{
		_newGameButton = GetNode<Button>("%newGameButton");
		_newGameButton.Pressed += OnNewGameButtonPressed;
	}

	private void OnNewGameButtonPressed()
	{
		var gameScene = GD.Load<PackedScene>("res://gameView.tscn");
		var game = gameScene.Instantiate() as GameView;
		AddChild(game);
		game.StartNewGame();
		_newGameButton.Hide();
	}
}
