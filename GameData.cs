using Godot;

public partial class GameData : Node
{
    public static GameData Instance { get; private set; }

    public int Player1Score = 0;
    public int Player2Score = 0;
    public string Winner = "";

    public override void _Ready()
    {
        Instance = this;
    }
}