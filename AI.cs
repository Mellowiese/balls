using Godot;

public partial class AI : CharacterBody2D
{
	[Export] private float speed = 250f;
	[Export] private float topLimit = -141.5f;
	[Export] private float bottomLimit = 125.5f;

	// Paddle stops chasing when within this many pixels of the ball's Y.
	[Export] private float deadZone = 15f;

	// Y position to drift back to when the ball is moving away.
	[Export] private float restY = -10f;

	// How fast the paddle drifts back to rest when the ball is moving away.
	[Export] private float driftSpeed = 80f;

	// Tracking error: the AI aims at the ball's Y plus a small offset that
	// shifts every few seconds, making it imperfect without being random
	// every frame.
	[Export] private float maxTrackingError = 10f;
	[Export] private float errorChangeInterval = 1.2f;

	private CharacterBody2D _ball;
	private float _trackingError = 0f;
	private float _errorTimer = 0f;

	// Cache the ball's previous X so we can derive its horizontal direction.
	private float _prevBallX = 0f;

	public override void _Ready()
	{
		_ball = GetNode<CharacterBody2D>("../Ball");
		_prevBallX = _ball.GlobalPosition.X;

		// Start with a random error offset so it doesn't look robotic from frame 1.
		_trackingError = (float)GD.RandRange(-maxTrackingError, maxTrackingError);
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		// --- Update tracking error on a slow timer ---
		_errorTimer -= dt;
		if (_errorTimer <= 0f)
		{
			_trackingError = (float)GD.RandRange(-maxTrackingError, maxTrackingError);
			_errorTimer = errorChangeInterval + (float)GD.RandRange(-0.3, 0.3);
		}

		// --- Determine ball's horizontal direction ---
		float ballX = _ball.GlobalPosition.X;
		float ballMovingRight = ballX - _prevBallX;  // positive = moving right = toward AI
		_prevBallX = ballX;

		float targetY;

		if (ballMovingRight >= 0f)
		{
			// Ball is moving toward the AI — chase it with an imperfect offset.
			targetY = _ball.GlobalPosition.Y + _trackingError;
		}
		else
		{
			// Ball is moving away — drift back to rest position.
			targetY = restY;
		}

		// --- Move toward targetY ---
		float currentY = GlobalPosition.Y;
		float diff = targetY - currentY;

		float moveSpeed;

		if (Mathf.Abs(diff) <= deadZone)
		{
			// Inside the dead zone: stop.
			moveSpeed = 0f;
		}
		else if (ballMovingRight < 0f)
		{
			// Drifting back — use the slower drift speed.
			moveSpeed = driftSpeed * Mathf.Sign(diff);
		}
		else
		{
			// Chasing — use full AI speed.
			moveSpeed = speed * Mathf.Sign(diff);
		}

		Vector2 velocity = Velocity;
		velocity.Y = moveSpeed;
		Velocity = velocity;

		MoveAndSlide();

		// Clamp position within court limits.
		Vector2 pos = Position;
		pos.Y = Mathf.Clamp(pos.Y, topLimit, bottomLimit);
		Position = pos;
	}
}
