using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SceneManager : Node
{
	public Dictionary<string, GameData> SceneSnapshots = new Dictionary<string, GameData>();
	private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();

	public static SceneManager Instance {get; private set;}

	public bool isReloading = false;

	public void SaveSceneState(string scenePath)
    {
		GameData currentSnapshot = new GameData();
		currentSnapshot.CaptureState(GetTree().GetNodesInGroup("Ball").Cast<Ball>().Where(n => n.isActive).ToArray(),
									 GetTree().GetNodesInGroup("Paddles").Cast<PlayerData>().ToArray(),
									 GetTree().GetNodesInGroup("Marker").Cast<Marker>().FirstOrDefault());
		GD.Print($"Saving state for scene {scenePath}: Ball at {currentSnapshot.ballSaves[0].Position} with velocity {currentSnapshot.ballSaves[0].Velocity}, Paddle 0 at {currentSnapshot.paddlePositions[0]}, Paddle 1 at {currentSnapshot.paddlePositions[1]}, Scores: {currentSnapshot.scores[0]} - {currentSnapshot.scores[1]}");
        SceneSnapshots[scenePath] = currentSnapshot;
    }

	public void LoadSceneState()
    {
		if(GetTree().CurrentScene == null)
		{
			GD.Print("Current scene is null");
			return;
		}

		if(GetTree().CurrentScene.SceneFilePath == null)
		{
			GD.Print("Current scene file path is null");
			return;
		}

    	if(SceneSnapshots.ContainsKey(GetTree().CurrentScene.SceneFilePath))
		{
			GameData snapshot = SceneSnapshots[GetTree().CurrentScene.SceneFilePath];
			if (snapshot != null)
			{
				snapshot.ApplyState(GetTree().GetNodesInGroup("Ball").Cast<Ball>().ToArray(), 
									GetTree().GetNodesInGroup("Paddles").Cast<PlayerData>().ToArray(), 
									GetTree().GetNodesInGroup("Marker").Cast<Marker>().FirstOrDefault());

				GD.Print($"Loaded state for scene {GetTree().CurrentScene.SceneFilePath}: Ball at {snapshot.ballSaves[0].Position} with velocity {snapshot.ballSaves[0].Velocity}, Paddle 0 at {snapshot.paddlePositions[0]}, Paddle 1 at {snapshot.paddlePositions[1]}, Scores: {snapshot.scores[0]} - {snapshot.scores[1]}");
			}

			GetTree().Paused = false;
		}
		else
		{
			GD.Print($"No saved state found for scene {GetTree().CurrentScene.SceneFilePath}");
		}	
    }

	public async void SwapScene(string scenePath, bool saveScene)
	{
    	if (saveScene && GetTree().CurrentScene != null)
        	SaveSceneState(GetTree().CurrentScene.SceneFilePath);

    	await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

    	GetTree().ChangeSceneToFile(scenePath);

    	await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

    	LoadSceneState();

    	GD.Print($"Swapped to scene {scenePath}");
	}

	public override void _Ready()
	{
		Instance = this;
	}

	public void UpdateCanvasGroups(CanvasLayer menuCanvasLayer)
	{
		canvasGroups.Clear();
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
