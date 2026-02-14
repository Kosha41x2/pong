using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class RedBall : Ball
{
	private bool isOnFire = false;

	[Export] private float maxSpeedPercentage = 0.75f;
	[Export] private int heatDamage = 2;
	protected override void HandlePaddleCollision(CharacterBody2D paddle)
	{
		base.HandlePaddleCollision(paddle);

		if (isOnFire)
		{
			PointsOfValue = 0;
			speed = maxSpeed * maxSpeedPercentage;
			paddle.GetNode<DamageHandle>("TakeDamage").TakeDamage(heatDamage);
			Deactivate();
		}
	}
}
