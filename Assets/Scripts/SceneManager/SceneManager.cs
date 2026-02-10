using Godot;
using System;
using System.Collections.Generic;

public partial class SceneManager : Node
{
	private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();

	public static SceneManager Instance {get; private set;}

	public bool isReloading = false;

	public override void _Ready()
	{
		Instance = this;
	}

	public void UpdateCanvasGroups(CanvasLayer menuCanvasLayer)
	{
		foreach (Node child in menuCanvasLayer.GetChildren())
		{
			if (child is CanvasGroup canvasGroup)
			{
				canvasGroups.Add(canvasGroup);
			}
		}
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

		isReloading = false;
	}

	public void OnExit()
	{
		GetTree().Quit();
	}

	public void OnRestart()
	{
		GetTree().Paused = false;
		canvasGroups.Clear();
		GetTree().ReloadCurrentScene();
		isReloading = true;
		GetTree().Paused = false;
	}
}
