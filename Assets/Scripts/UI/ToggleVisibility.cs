using Godot;
using System;

public partial class ToggleVisibility : Control
{
	public override void _Ready()
	{
		Visible = Settings.Instance.visibleControls;
		Settings.Instance.UpdateValue += OnUpdateValue;
	}
	
	public override void _ExitTree()
	{
		Settings.Instance.UpdateValue -= OnUpdateValue;
	}
	private void OnUpdateValue()
	{
		Visible = Settings.Instance.visibleControls;
	}
}
