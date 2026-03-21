using Godot;
using System;
using System.Drawing;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;

public partial class Ball : CharacterBody2D
{
    [Export] public float initialSpeed {get; protected set;} = 400f;

	[Export] public int Weight {get; protected set;}
	protected int directionValue;
	RandomNumberGenerator rng = new RandomNumberGenerator();
	protected float speed = 400f;

	protected float maxSpeed = 1200f;
	protected float speedIncreaseFactor = 1.05f;

	protected float bounceMaxAngle = 45f;

	[Export] public int PointsOfValue { get; protected set; } = 1;

	public bool isActive {get; protected set;} = false;

	public bool isFromOtherSene {get; set;} = false;

	public override void _Ready()
	{
		Settings.Instance.UpdateValue += OnValueUpdated;
		OnValueUpdated();
	}
	public override void _ExitTree()
	{
		Settings.Instance.UpdateValue -= OnValueUpdated;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!isActive) return;
		Velocity = Velocity.Normalized() * speed;
		CollisionLogic(delta);
	}

	private void CollisionLogic(double delta)
	{
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

	protected virtual void HandlePaddleCollision(CharacterBody2D paddle)
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

    public void ResetBall(Vector2 startPosition, Vector2? initialVelocity = null)
	{
		if (isFromOtherSene)
		{
			isFromOtherSene = false;
			speed = Velocity.Length();
			GD.Print("Ball is from other scene, not resetting position and velocity.");
			return;
		}

		Position = startPosition;
		directionValue = rng.RandiRange(0, 1);
		
		directionValue = directionValue == 1 ? 1 : -1;

		if(initialVelocity != null)
		{
			Velocity = initialVelocity.Value;
		}
		else
		{
			Velocity = initialSpeed * directionValue * Vector2.Right;
        	speed = initialSpeed;
		}
	}

	virtual protected void OnValueUpdated()
	{
		maxSpeed = Settings.Instance.maxSpeed;
		speedIncreaseFactor = Settings.Instance.speedIncreaseFactor;
		bounceMaxAngle = Settings.Instance.bounceMaxAngle;

		GD.Print("Settings Updated: Max Speed: " + maxSpeed + ", Speed Increase Factor: " + speedIncreaseFactor + ", Bounce Max Angle: " + bounceMaxAngle);
	}

	public void Activate(Vector2 startPosition, Vector2? initialVelocity = null)
	{    
    	ResetBall(startPosition, initialVelocity); 
    
    	Visible = true;

		isActive = true;

		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", false);
	}

	public void ActivateAfterDelay(Vector2 startPosition, float delay)
	{
		Task.Run(async () =>
		{
			await Task.Delay(TimeSpan.FromSeconds(delay));
			GD.Print("Activating ball after delay of " + delay + " seconds.");
			CallDeferred(nameof(Activate), startPosition);
		});
	}

	public virtual void Deactivate()
	{
		isActive = false;

    	Visible = false;

		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);
	}
}