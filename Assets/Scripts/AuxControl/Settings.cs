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

	[Export] public bool visibleControls = true;

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
}
