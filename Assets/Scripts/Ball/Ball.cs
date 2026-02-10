using Godot;
using System;
using System.Drawing;
using System.Threading.Tasks;

public partial class Ball : CharacterBody2D
{
    [Export] private float initialSpeed = 400f;
	private int directionValue;
	RandomNumberGenerator rng = new RandomNumberGenerator();
	private float speed = 400f;

	private float maxSpeed = 1200f;
	private float speedIncreaseFactor = 1.05f;

	private float bounceMaxAngle = 45f;

	public override async void _Ready()
    {
		ResetBall();
		Settings.Instance.UpdateValue += OnValueUpdated;
    }

	public override void _PhysicsProcess(double delta)
	{
		Velocity = Velocity.Normalized() * speed;
		CollisionLogic(delta);
	}

	private void CollisionLogic(double delta)
	{
		GD.Print("Ball Speed: " + speed);
		KinematicCollision2D collision = MoveAndCollide(Velocity * (float)delta);

		if (collision != null)
		{
			GD.Print("Ball Collided with: " + collision.GetCollider().GetType().ToString());

			if (collision.GetCollider() is CharacterBody2D characterBody2D && characterBody2D.IsInGroup("Paddles") && (collision.GetNormal() == Vector2.Left || collision.GetNormal() == Vector2.Right))
			{
				HandlePaddleCollision(characterBody2D);
			}
			else
			{
				Velocity = Velocity.Bounce(collision.GetNormal());
			}
		}
	}

	private void HandlePaddleCollision(CharacterBody2D paddle)
	{
		float bouncePercentage = CalculateBounceAngle(paddle);

		float bounceDir = (Velocity.X > 0) ? -1 : 1;

		float currentAngle = Mathf.Lerp(-bounceMaxAngle, bounceMaxAngle, (bouncePercentage + 1f) / 2f);

		Vector2 newDir = new Vector2(bounceDir * Mathf.Cos(Mathf.DegToRad(currentAngle)), Mathf.Sin(Mathf.DegToRad(currentAngle))).Normalized();

		GD.Print("Data: " + "Bounce Percentage: " + bouncePercentage + ", Bounce Direction: " + bounceDir + ", Current Angle: " + currentAngle);

		Velocity = newDir * speed;

		speed = Math.Min(speed * speedIncreaseFactor, maxSpeed);
	}

	private float CalculateBounceAngle(CharacterBody2D paddle)
	{
		CollisionShape2D collisionShape2D = paddle.GetNode<CollisionShape2D>("CollisionShape2D");
		float relativeHitPos = (GlobalPosition.Y - paddle.GlobalPosition.Y);

		CollisionShape2D ballCollisionShape2D = this.GetNode<CollisionShape2D>("CollisionShape2D");
		float normalizedHit = relativeHitPos / (collisionShape2D.Shape.GetRect().Size.Y * collisionShape2D.GlobalScale.Y / 2 + ballCollisionShape2D.Shape.GetRect().Size.Y * ballCollisionShape2D.GlobalScale.Y / 2);
		GD.Print("Relative Hit Position: " + relativeHitPos + ", Normalized Hit: " + normalizedHit + "max distance:" + ((collisionShape2D.Shape.GetRect().Size.Y * paddle.Scale.Y) / 2 + (this.GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size.Y * Scale.Y) / 2));
		return normalizedHit;
	}

    public void ResetBall()
	{
		Position = Vector2.Zero;
		directionValue = rng.RandiRange(0, 1);
		
		directionValue = directionValue == 1 ? 1 : -1;

		Velocity = initialSpeed * directionValue * Vector2.Right;
        speed = initialSpeed;
	}

	private void OnValueUpdated()
	{
		maxSpeed = Settings.Instance.maxSpeed;
		speedIncreaseFactor = Settings.Instance.speedIncreaseFactor;
		bounceMaxAngle = Settings.Instance.bounceMaxAngle;
	}
}