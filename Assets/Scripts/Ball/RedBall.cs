using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class RedBall : Ball
{
	private bool isOnFire = false;

	[Export] private float maxSpeedPercentage = 0.75f;
	[Export] private int heatDamage = 2;

	public override void _Ready()
	{
		base._Ready();
		PointsOfValue = 1;
		isOnFire = false;
	}

    public override void Deactivate()
    {
        base.Deactivate();
        isOnFire = false;
    }

	protected override void HandlePaddleCollision(CharacterBody2D paddle)
	{
		base.HandlePaddleCollision(paddle);
		PointsOfValue = 0;
		speed = maxSpeed * maxSpeedPercentage;

		if (isOnFire)
		{
			paddle.GetNode<DamageHandle>("DamageHandler").TakeDamage(heatDamage);
			Deactivate();
			BallRandomizerManager.Instance.GetRandomBall().Activate(new Vector2(0, 0));
			return;
		}

		isOnFire = true;
	}
}
