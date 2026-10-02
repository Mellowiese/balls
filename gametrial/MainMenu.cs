using Godot;
using System;

public partial class MainMenu : Control
{
	private void OnPlayButtonPressed()
	{
		GD.Print("Play button clicked!");
		GetTree().ChangeSceneToFile("res://gametrial/game.tscn");
	}

	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}
}
