using Godot;
using System;

public partial class SceneManager : Node
{
	private static SceneManager _instance;

	[Export] CanvasGroup settingsMenu;

	public static SceneManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new SceneManager();
			}
			return _instance;
		}
	}

	public override void _Ready()
	{
		if (_instance != null && _instance != this)
		{
			QueueFree();
			return;
		}
		_instance = this;
	}

	public void SettingsToggle(bool value)
	{
		GD.Print("Settings Toggle: " + value);
		settingsMenu.Visible = value;
		GetTree().Paused = value;
	}
}
