using Godot;
using System;
using System.ComponentModel;
using System.Reflection.Metadata;
using Godot.Collections;
using System.Linq;

public partial class Settings : Node2D
{
	public static Settings Instance {get; private set;}

	[Signal] public delegate void UpdateValueEventHandler();

	private Array<Node> balls = new Array<Node>();

	public async override void _Ready()
	{
		Instance = this;
		
		EmitSignal(nameof(UpdateValue));
	}

	[Export] public float initialSpeed = 400f;
	[Export] public float speedIncreaseFactor = 1.05f;
	[Export] public float maxSpeed = 1600f;
	[Export] public float bounceMaxAngle = 45f;

	[Export] public bool visibleControls = true;

	[Export] public Array<int> ballsWeights = new Array<int>(){50, 10, 10, 10, 10};

	public void OnMaxSpeedChange(float value)
	{
		maxSpeed = value;
		EmitSignal(nameof(UpdateValue));
	}

	public void OnMaxAngleChange(float value)
	{
		bounceMaxAngle = value;
		EmitSignal(nameof(UpdateValue));
	}

	public void OnSpeedIncreaseFactorChange(float value)
	{
		speedIncreaseFactor = value/100f + 1f;
		EmitSignal(nameof(UpdateValue));
	}

	public void OnVisibleControlsToggled(bool value)
	{
		visibleControls = value;
		EmitSignal(nameof(UpdateValue));
	}
	public void OnBallWeightChange(int index, int value)
	{
		if(index >= 0 && index < ballsWeights.Count)
		{
			ballsWeights[index] = value;
			EmitSignal(nameof(UpdateValue));
		}
	}
}
