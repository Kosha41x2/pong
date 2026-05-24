using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class GameData : Resource
{
	[Export] public int[] scores = new int[2];
	public List<BallSave> ballSaves = new List<BallSave>();
	[Export] public Vector2[] paddlePositions = new Vector2[2];

	public void CaptureState(Ball[] balls, PlayerData[] paddles, Marker marker)
	{
		for(int i = 0; i < scores.Length; i++)
		{
			scores[i] = marker.GetScore(i);
		}

		for (int i = 0; i < balls.Length; i++)
		{
			if (i >= ballSaves.Count)
			{
				ballSaves.Add(new BallSave());
			}
			ballSaves[i].CaptureState(balls[i]);
		}
		foreach (PlayerData paddle in paddles)
		{
			paddlePositions[paddle.PlayerN] = paddle.Position;
		}
	}

	public void ApplyState(Ball[] balls, PlayerData[] paddles, Marker marker)
	{
		for(int i = 0; i < scores.Length; i++)
		{
			marker.SetScore(i, scores[i]);
		}

		foreach (Ball ball in balls)
		{
			foreach (BallSave ballSave in ballSaves)
			{
				if (ballSave.Ball.GetType() == ball.GetType())
				{
					ball.Position = ballSave.Position;
					ball.Velocity = ballSave.Velocity;
					ball.isFromOtherSene = true;

					BallRandomizerManager.Instance.DeactivateAllBalls();
					ball.Activate(ball.Position, ball.Velocity);
					GD.Print($"Applying state to ball: Position {ball.Position}, Velocity {ball.Velocity}");
					break;
				}
			}
		}

		foreach (PlayerData paddle in paddles)
		{
			paddle.Position = paddlePositions[paddle.PlayerN];
		}
	}
}
