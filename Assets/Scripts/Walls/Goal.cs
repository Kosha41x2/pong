using Godot;
using System;

public partial class Goal : Area2D
{
	[Signal] public delegate void GoalReachedEventHandler(int score, int playerN);

	[Export] public int PlayerNumber { get; set; } = 0;
	public override void _Ready()
	{
		Connect("body_entered", new Callable(this, nameof(OnBodyEntered)));
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.IsInGroup("Ball") && body is Ball ball)
		{
			EmitSignal(SignalName.GoalReached, 1, PlayerNumber);
			ball.ResetBall();
		}
	}
}
