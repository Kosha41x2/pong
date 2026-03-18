using Godot;
using System;

public partial class BallSave : Node
{
	[Export] public Ball Ball {get; private set;}

	[Export] public Vector2 Position {get; private set;}
	[Export] public Vector2 Velocity {get; private set;}

	public void CaptureState(Ball ball)
	{
		Ball = ball;
		Position = ball.Position;
		Velocity = ball.Velocity;
	}
}
