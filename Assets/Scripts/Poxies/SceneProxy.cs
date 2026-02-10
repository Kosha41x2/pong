using Godot;
using System;
using System.ComponentModel;

public partial class SceneProxy : Node
{
	[Export] private CanvasLayer canvasLayer;

	[Signal] public delegate void ToggleInitialButtonEventHandler(bool value);

	public override void _Ready()
	{
		SceneManager.Instance.UpdateCanvasGroups(canvasLayer);
		if(SceneManager.Instance.isReloading) EmitSignal(nameof(ToggleInitialButton), true);
	}
	

	public void OnMenuRequest(CanvasGroup canvasGroup, bool value)
	{
		SceneManager.Instance.OnMenuRequest(canvasGroup, value);
	}

	private void OnExit()
	{
		SceneManager.Instance.OnExit();
	}

	private void OnRestart()
	{
		SceneManager.Instance.OnRestart();
	}
}
