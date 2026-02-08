using Godot;
using System;

public partial class ToggleVisibility : Control
{
	private void OnToggleVisibilityButtonPressed(bool value)
	{
		Visible = value;
	}
}
