using Godot;

public partial class Results : Control
{
    private Label _winnerLabel;
    private Label _finalScore;
    private BaseButton _mainMenuButton;
    private BaseButton _retryButton;

    public override void _Ready()
    {
        // Hook up nodes (must match exact node names in results.tscn)
        _winnerLabel    = GetNode<Label>("WinnerLabel");
        _finalScore     = GetNode<Label>("FinalScore");
        _mainMenuButton = GetNode<BaseButton>("MainMenuButton");
        _retryButton    = GetNode<BaseButton>("RetryButton");

        // Pull scores from the singleton
        _winnerLabel.Text = GameData.Instance.Winner + " Wins!";
        _finalScore.Text  = $"{GameData.Instance.Player1Score} - {GameData.Instance.Player2Score}";

        // Wire buttons
        _mainMenuButton.Pressed += OnMainMenuPressed;
        _retryButton.Pressed    += OnRetryPressed;
    }

    private void OnMainMenuPressed()
    {
        GameData.Instance.Player1Score = 0;
        GameData.Instance.Player2Score = 0;
        GameData.Instance.Winner = "";
        GetTree().ChangeSceneToFile("res://gametrial/main_menu.tscn");
    }

    private void OnRetryPressed()
    {
        GameData.Instance.Player1Score = 0;
        GameData.Instance.Player2Score = 0;
        GameData.Instance.Winner = "";
        GetTree().ChangeSceneToFile("res://gametrial/game.tscn");
    }
}