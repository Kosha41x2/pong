using Godot;
using System;
using System.Reflection;

public partial class SceneSwapper : Node
{
	[Signal] public delegate void SceneRequestEventHandler(string scenePath, bool saveScene);
	[Export] private string scenePath;
	[Export] private bool saveSceneOnSwap = true;
	
	public void RequestSceneSwap()
	{
		GD.Print($"Requesting scene swap to {scenePath}");
		EmitSignal(nameof(SceneRequest), scenePath, saveSceneOnSwap);
	}
}
