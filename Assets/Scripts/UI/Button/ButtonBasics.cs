using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;

public partial class ButtonBasics : Button
{
	[Signal] public delegate void CanvasRequestEventHandler(CanvasGroup canvasGroup, bool value);

	[Export] private CanvasGroup canvasGroup;
	[Export] private string initialText= "Click Me!";
	[Export] private string changeText = "Clicked!";

	public override void _Ready()
	{
		Text = initialText;	
	}

	private void OnTextChanged(bool value)
	{
		if(value) Text = changeText;
		else Text = initialText;
	}

	private void OnCanvasRequest(bool value)
	{
		EmitSignal(nameof(CanvasRequest), canvasGroup, value);
	}

	private void OnCanvasRequest()
	{
		EmitSignal(nameof(CanvasRequest), canvasGroup, true);
	}

	private void SetButtonPressed(bool value)
	{
		this.ButtonPressed = value;
		OnTextChanged(value);
		if(canvasGroup != null) OnCanvasRequest(value);
	}
}
