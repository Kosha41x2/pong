using Godot;
using System;
using System.Collections.Generic;

public partial class SceneManager : Node
{
	private static SceneManager _instance;

	[Export] public CanvasGroup[] canvasGroups = new CanvasGroup[] { };

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

	public void OnMenuRequest(CanvasGroup canvasGroup, bool value)
	{
		foreach (CanvasGroup group in canvasGroups)
		{
			group.Visible = false;
		}

		canvasGroup.Visible = value;

		if(canvasGroup.Visible) GetTree().Paused = true;
		else GetTree().Paused = false;
	}

	private void OnExit()
	{
		GetTree().Quit();
	}
}
