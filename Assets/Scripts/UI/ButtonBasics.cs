using Godot;
using System;

public partial class ButtonBasics : Button
{
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
}
