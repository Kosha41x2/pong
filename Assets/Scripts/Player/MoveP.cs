using Godot;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class MoveP : Node2D
{
	[Export] private float speed = 400.0f;
	[Export] private string inputSuffix = "p1";

	private PlayerData parentBody;

	float velocityY = 0.0f;

	public override void _Ready()
	{
		parentBody = GetParent<PlayerData>();
		inputSuffix = "p" + (parentBody.PlayerN + 1).ToString();
	} 
    public override void _PhysicsProcess(double delta)
    {
		float inputDir = Input.GetActionStrength("move_down_" + inputSuffix) - Input.GetActionStrength("move_up_" + inputSuffix);

		if (inputDir != 0)
		{
			velocityY = inputDir * speed;
		}
		else
		{
			velocityY = 0.0f;
		}

		parentBody.Velocity = new Vector2(0.0f, velocityY);
		parentBody.MoveAndSlide();
	}
}
