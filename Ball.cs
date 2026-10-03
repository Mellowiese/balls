using Godot;

public partial class Ball : CharacterBody2D
{
	[Export] public float StartSpeed = 300f;
	[Export] public float SpeedIncrease = 20f;
	[Export] public float MaxSpeed = 600f;

	// Maximum bounce angle in degrees when hitting the very edge of a paddle.
	[Export] public float MaxBounceAngleDeg = 60f;

	// Pause (seconds) before the ball launches after a reset.
	[Export] public float ServeDelay = 0.8f;

	private float _currentSpeed;
	private Vector2 _direction = Vector2.Zero;
	private bool _launched = false;
	private Vector2 _startPosition;

	private Game _game;

	public override void _Ready()
	{
		_game = GetParent<Game>();
		_startPosition = GlobalPosition;

		// MoveAndCollide never reports Area2D nodes, so goals are detected
		// with the Area2D BodyEntered signal instead.
		GetNode<Area2D>("../LeftGoal").BodyEntered += OnLeftGoalBodyEntered;
		GetNode<Area2D>("../RightGoal").BodyEntered += OnRightGoalBodyEntered;

		ResetBall(1);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_launched)
			return;

		Vector2 motion = _direction * _currentSpeed * (float)delta;
		KinematicCollision2D collision = MoveAndCollide(motion);

		if (collision == null)
			return;

		GodotObject collider = collision.GetCollider();

		if (collider is StaticBody2D)
		{
			// Top or bottom wall: reflect off the surface normal.
			_direction = _direction.Bounce(collision.GetNormal());
		}
		else if (collider is CharacterBody2D paddle)
		{
			// Use the paddle's real collision shape so the bounce angle
			// stays correct if the paddle size changes.
			CollisionShape2D shapeNode = paddle.GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
			float halfHeight = 44f;
			float centerY = paddle.GlobalPosition.Y;

			if (shapeNode != null && shapeNode.Shape is RectangleShape2D rect)
			{
				halfHeight = rect.Size.Y / 2f;
				centerY = shapeNode.GlobalPosition.Y;
			}

			float normalized = Mathf.Clamp((GlobalPosition.Y - centerY) / halfHeight, -1f, 1f);
			float bounceAngle = normalized * Mathf.DegToRad(MaxBounceAngleDeg);

			// Always leave the paddle heading away from it.
			float dirX = paddle.GlobalPosition.X < GlobalPosition.X ? 1f : -1f;

			_direction = new Vector2(
				Mathf.Cos(bounceAngle) * dirX,
				Mathf.Sin(bounceAngle)
			).Normalized();

			_currentSpeed = Mathf.Min(_currentSpeed + SpeedIncrease, MaxSpeed);
		}
	}

	private void OnLeftGoalBodyEntered(Node2D body)
	{
		if (body != this || !_launched)
			return;

		_game.AddScore(2);   // ball got past Player 1 (left), so Player 2 scores
		ResetBall(1);
	}

	private void OnRightGoalBodyEntered(Node2D body)
	{
		if (body != this || !_launched)
			return;

		_game.AddScore(1);   // ball got past the AI (right), so Player 1 scores
		ResetBall(-1);
	}

	/// <summary>
	/// Puts the ball back at its start position, then launches it after a short delay.
	/// serveDirection: -1 = left, 1 = right.
	/// </summary>
	public async void ResetBall(int serveDirection)
	{
		if(!IsInsideTree())
			return;

		_launched = false;
		Velocity = Vector2.Zero;
		GlobalPosition = _startPosition;
		_currentSpeed = StartSpeed;

		await ToSignal(GetTree().CreateTimer(ServeDelay), SceneTreeTimer.SignalName.Timeout);

		// The scene may have changed (e.g. game over) while we were waiting.
		if (!IsInstanceValid(this) || !IsInsideTree())
			return;

		float angle = (float)GD.RandRange(
			Mathf.DegToRad(-30.0),
			Mathf.DegToRad(30.0)
		);

		_direction = new Vector2(
			Mathf.Cos(angle) * serveDirection,
			Mathf.Sin(angle)
		).Normalized();

		_launched = true;
	}
}
