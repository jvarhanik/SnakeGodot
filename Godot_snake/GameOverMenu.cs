using Godot;
using System;

public partial class GameOverMenu : Control
{
	public Label scoreLabel;
	public override void _Ready()
	{
		scoreLabel = GetNode<Label>("ScoreLabel");
		Button tryAgainButton = GetNode<Button>("TryAgainButton");
		Button quitButton = GetNode<Button>("QuitButton");

		tryAgainButton.Connect("pressed", new Callable(this, nameof(OnTryAgainPressed)));
		quitButton.Connect("pressed", new Callable(this, nameof(OnQuitPressed)));
	}

	private void OnTryAgainPressed()
	{
		GetTree().ReloadCurrentScene();
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}

	public void UpdateScore(int score)
	{
		scoreLabel.Text = "Your Score: " + score.ToString();
	}

}
