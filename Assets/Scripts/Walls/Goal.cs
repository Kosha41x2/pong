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
    	GD.Print("Goal reached by: " + body.Name + " in Player " + PlayerNumber);
    	if (body.IsInGroup("Ball") && body is Ball ball && ball.isActive)
    	{
			GD.Print("Ball is in" + ball.Position);
			Ball randomBall = BallRandomizerManager.Instance.GetRandomBall();
			EmitSignal(nameof(GoalReached), ball.PointsOfValue, PlayerNumber);
			ball.Deactivate();
			randomBall.Activate(new Vector2(0, 0));
    	}
	}
}
