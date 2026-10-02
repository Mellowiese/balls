using Godot;

public partial class Game : Node2D
{
    private const int WIN_SCORE = 5;

    private Label _p1ScoreLabel;
    private Label _p2ScoreLabel;

    public override void _Ready()
    {
        _p1ScoreLabel = GetNode<Label>("UI/Player 1 Score");
        _p2ScoreLabel = GetNode<Label>("UI/Player 2 Score");

        UpdateScoreDisplay();
    }

    // Your teammates call this when the ball passes a paddle
    public void AddScore(int player)
    {
        if (player == 1)
            GameData.Instance.Player1Score++;
        else
            GameData.Instance.Player2Score++;

        UpdateScoreDisplay();
        CheckWinCondition();
    }

    private void UpdateScoreDisplay()
    {
        _p1ScoreLabel.Text = GameData.Instance.Player1Score.ToString();
        _p2ScoreLabel.Text = GameData.Instance.Player2Score.ToString();
    }

    private void CheckWinCondition()
    {
        if (GameData.Instance.Player1Score >= WIN_SCORE)
        {
            GameData.Instance.Winner = "Player 1";
            GoToResults();
        }
        else if (GameData.Instance.Player2Score >= WIN_SCORE)
        {
            GameData.Instance.Winner = "Player 2";
            GoToResults();
        }
    }

    private void GoToResults()
    {
        GetTree().ChangeSceneToFile("res://gametrial/results.tscn");
    }
}