using Godot;

public partial class Player1 : CharacterBody2D
{
	[Export] private float speed = 400f;
	[Export] private float topLimit = -141.5f;    // board's inner top edge Y
	[Export] private float bottomLimit = 125.5f; // board's inner bottom edge Y

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		velocity.Y = 0;

		if (Input.IsActionPressed("move_up"))
			velocity.Y = -speed;
		else if (Input.IsActionPressed("move_down"))
			velocity.Y = speed;

		Velocity = velocity;
		MoveAndSlide();

		// Clamp position so paddle can't leave the board
		Vector2 pos = Position;
		pos.Y = Mathf.Clamp(pos.Y, topLimit, bottomLimit);
		Position = pos;
	}
}
