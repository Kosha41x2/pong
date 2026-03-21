using Godot;
using System;

public partial class GreenBall : Ball
{
	protected override void OnValueUpdated()
	{
		base.OnValueUpdated();

		speedIncreaseFactor = (speedIncreaseFactor - 1) * 2 + 1;
	}
}
