using Godot;
using System;
using Godot.Collections;
using System.Runtime.CompilerServices;
public partial class BallRandomizerManager : Node2D
{
	public static BallRandomizerManager Instance {get; private set;}
	[Export] public Array<Ball> balls = new Array<Ball>();

	[Export] private Ball firstBall;
	private Random random = new Random();

    public override void _Ready()
    {
		Instance = this;
		Reset();
    }

	public void Reset()
	{
		foreach(Ball ball in balls)
		{
			ball.Deactivate();
		}
		firstBall.Activate(new Vector2(0,0));
	}

	public Ball GetRandomBall()
	{
		if(balls.Count == 0)
		{
			GD.PrintErr("No balls in BallRandomizerManager");
			return firstBall;
		}

		int maxWeight = GetMaxWeight();

		Ball ballPicked = firstBall;

		int randomN;

		randomN = maxWeight > 0? random.Next(1, maxWeight) : 1;

		int weightSum = 0;
		foreach (Ball ball in balls)
		{
			weightSum += ball.Weight;
			if(weightSum >= randomN)
			{
				ballPicked = ball;
				break;
			}
		}
		return ballPicked;
	}

	public void DeactivateAllBalls()
	{
		foreach(Ball ball in balls)
		{
			ball.Deactivate();
		}
	}

	private int GetMaxWeight()
	{
		int weightSum = 0;
		foreach (Ball ball in balls)
		{
			weightSum += ball.Weight;
		}
		return weightSum;
	}

	public void SetFistBall(Ball ball)
	{
		firstBall = ball;
	}
}
