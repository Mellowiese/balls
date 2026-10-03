using Godot;

public partial class PauseMenu : Control
{
	private BaseButton _pauseButton;
	private BaseButton _resumeButton;
	private BaseButton _restartButton;
	private BaseButton _exitButton;

	public override void _Ready()
	{
		// Keep this node processing even while the tree is paused.
		ProcessMode = ProcessModeEnum.Always;

		// Start hidden.
		Visible = false;

		// Grab buttons. PauseMenu lives under UI, so the pause button is a sibling.
		_pauseButton  = GetNode<BaseButton>("../TextureButton");
		_resumeButton  = GetNode<BaseButton>("VBoxContainer/ResumeButton");
		_restartButton = GetNode<BaseButton>("VBoxContainer/RestartButton");
		_exitButton    = GetNode<BaseButton>("VBoxContainer/ExitButton");

		_pauseButton.Pressed  += OnPausePressed;
		_resumeButton.Pressed  += OnResumePressed;
		_restartButton.Pressed += OnRestartPressed;
		_exitButton.Pressed    += OnExitPressed;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event.IsActionPressed("ui_cancel"))
		{
			if (GetTree().Paused)
				Resume();
			else
				Pause();

			// Consume the event so nothing else reacts to Escape.
			GetViewport().SetInputAsHandled();
		}
	}

	// -------------------------------------------------------------------------
	// Helpers
	// -------------------------------------------------------------------------

	private void Pause()
	{
		GetTree().Paused = true;
		Visible = true;
	}

	private void Resume()
	{
		GetTree().Paused = false;
		Visible = false;
	}

	private static void ResetGameData()
	{
		GameData.Instance.Player1Score = 0;
		GameData.Instance.Player2Score = 0;
		GameData.Instance.Winner = "";
	}

	// -------------------------------------------------------------------------
	// Button handlers
	// -------------------------------------------------------------------------

	private void OnPausePressed()
	{
		Pause();
	}

	private void OnResumePressed()
	{
		Resume();
	}

	private void OnRestartPressed()
	{
		ResetGameData();
		Resume();   // unpause BEFORE changing scenes
		GetTree().ChangeSceneToFile("res://gametrial/game.tscn");
	}

	private void OnExitPressed()
	{
		ResetGameData();
		Resume();   // unpause BEFORE changing scenes
		GetTree().ChangeSceneToFile("res://gametrial/main_menu.tscn");
	}
}
