using Godot;
using System;

public partial class YellowBall : Ball
{
	public override void _Ready()
	{
		base._Ready();
		PointsOfValue = 2;
	}
}
