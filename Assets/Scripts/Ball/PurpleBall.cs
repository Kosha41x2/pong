using Godot;
using System;

public partial class PurpleBall : Ball
{

	Random random = new Random();
    protected override void HandlePaddleCollision(CharacterBody2D paddle)
    {
        base.HandlePaddleCollision(paddle);

		speed = random.Next((int)initialSpeed, (int)maxSpeed);
    }
}
