using Godot;
using System;
using System.ComponentModel;

public partial class Settings : Node2D
{
	public static Settings Instance {get; private set;}

	[Signal] public delegate void UpdateValueEventHandler();

	public override void _Ready()
	{
		Instance = this;
		EmitSignal(nameof(UpdateValue));
	}

	[Export] public float initialSpeed = 400f;
	[Export] public float speedIncreaseFactor = 1.05f;
	[Export] public float maxSpeed = 1200f;
	[Export] public float bounceMaxAngle = 45f;

	private void OnMaxSpeedChange(float value)
	{
		maxSpeed = value;
		EmitSignal(nameof(UpdateValue));
	}

	private void OnMaxAngleChange(float value)
	{
		bounceMaxAngle = value;
		EmitSignal(nameof(UpdateValue));
	}

	private void OnSpeedIncreaseFactorChange(float value)
	{
		speedIncreaseFactor = value/100f + 1f;
		EmitSignal(nameof(UpdateValue));
		GD.Print("Speed Increase Factor: " + speedIncreaseFactor);
	}
}
