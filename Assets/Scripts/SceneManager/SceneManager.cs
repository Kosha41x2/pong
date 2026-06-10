using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SceneManager : Node
{
	[Export] string reset = "reset";
	public Dictionary<string, GameData> SceneSnapshots = new Dictionary<string, GameData>();

	public Dictionary<long, bool> playerShouldBePaused = new Dictionary<long, bool>();
	private List<CanvasGroup> canvasGroups = new List<CanvasGroup>();

	public static SceneManager Instance {get; private set;}

	public bool isReloading = false;

	public async void SaveSceneState(string scenePath)
    {
		GameData currentSnapshot = new GameData();
		currentSnapshot.CaptureState(GetTree().GetNodesInGroup("Ball").Cast<Ball>().Where(n => n.isActive).ToArray(),
									 GetTree().GetNodesInGroup("Paddles").Cast<PlayerData>().ToArray(),
									 GetTree().GetNodesInGroup("Marker").Cast<Marker>().FirstOrDefault());
		GD.Print($"Saving state for scene {scenePath}: Ball at {currentSnapshot.ballSaves[0].Position} with velocity {currentSnapshot.ballSaves[0].Velocity}, Paddle 0 at {currentSnapshot.paddlePositions[0]}, Paddle 1 at {currentSnapshot.paddlePositions[1]}, Scores: {currentSnapshot.scores[0]} - {currentSnapshot.scores[1]}");
        SceneSnapshots[scenePath] = currentSnapshot;
    }

	private void LoadSceneState(string scenePath)
    {
    	if(SceneSnapshots.ContainsKey(scenePath))
		{
			GameData snapshot = SceneSnapshots[scenePath];
			if (snapshot != null)
			{
				snapshot.ApplyState(GetTree().GetNodesInGroup("Ball").Cast<Ball>().ToArray(), 
									GetTree().GetNodesInGroup("Paddles").Cast<PlayerData>().ToArray(), 
									GetTree().GetNodesInGroup("Marker").Cast<Marker>().FirstOrDefault());

				GD.Print($"Loaded state for scene {scenePath}: Ball at {snapshot.ballSaves[0].Position} with velocity {snapshot.ballSaves[0].Velocity}, Paddle 0 at {snapshot.paddlePositions[0]}, Paddle 1 at {snapshot.paddlePositions[1]}, Scores: {snapshot.scores[0]} - {snapshot.scores[1]}");
			}
		}
		else
		{
			GD.Print($"No saved state found for scene {scenePath}");
		}	
    }

	public async void SwapScene(string scenePath, bool saveScene, bool pause)
	{
    	if (saveScene && GetTree().CurrentScene != null)
        	SaveSceneState(GetTree().CurrentScene.SceneFilePath);

    	await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

    	GetTree().ChangeSceneToFile(scenePath);

    	await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

    	LoadSceneState(scenePath);

		ServerManager._instance.Rpc(nameof(ServerManager.ReassignMultiplayerAuthority), -1); // Reassign authority to the first client or server after scene swap

		Rpc(nameof(ExecutePause), pause, Multiplayer.GetUniqueId()); // Pause the game if requested after scene swap

    	GD.Print($"Swapped to scene {scenePath}");
	}

	public async override void _Ready()
	{
		Instance = this;
		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged); // Wait for the scene to change before trying to find nodes to give authority to, since they might not be in the tree yet

		OnlineVSOffline._instance.OnlineModeActivated += SetInitialPauseState;
		OnlineVSOffline._instance.OfflineModeActivated += OnOfflineModeActivated;

		if(GetTree().CurrentScene != null)
		{
			SaveSceneState(reset); // Use a special key for the initial state
			GD.Print($"Initial scene state saved for {GetTree().CurrentScene.SceneFilePath} with key '{reset}'");
		}
	}

	public override void _ExitTree()
	{
		OnlineVSOffline._instance.OnlineModeActivated -= SetInitialPauseState;
		OnlineVSOffline._instance.OfflineModeActivated -= OnOfflineModeActivated;
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
		GD.Print($"Setting canvas group {canvasGroup.Name} visibility to {value}");
		canvasGroup.Visible = value;

		Rpc(nameof(RequestPause), value, Multiplayer.GetUniqueId());
		isReloading = false;
	}

	public void OnExit()
	{
		GetTree().Quit();
	}

	public void OnRestart()
	{
		Rpc(nameof(ResetGameState));
	}

	[Rpc(CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void ResetGameState()
	{
		GetTree().Paused = false;
		isReloading = true;
		LoadSceneState(reset);
		BallRandomizerManager.Instance.Reset();
		GetTree().Paused = true;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
	public void RequestPause(bool shouldPause, long senderId)
	{
		int [] peerIds = Multiplayer.GetPeers();

		playerShouldBePaused[senderId] = shouldPause;
		
    	if (!Multiplayer.IsServer()) return;

		if (!shouldPause)
		{
			foreach (var peerId in playerShouldBePaused.Keys.ToList())
			{
				if(playerShouldBePaused[peerId]) shouldPause = true; // If any player should be paused, the game remains paused
			}
		}

		if (OnlineVSOffline._instance != null && !OnlineVSOffline._instance.IsOffline && peerIds.Length <= 0)
		{
			shouldPause = true; // If we're online but there's only one player, we should pause until another player joins
		}

        Rpc(nameof(ExecutePause), shouldPause, senderId);
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	private void ExecutePause(bool shouldPause, long senderId)
	{
		if(!playerShouldBePaused.ContainsKey(senderId)) return; // If we don't have a record of this player's pause state, do nothing

    	GetTree().Paused = shouldPause;
		GD.Print($"Game {(shouldPause ? "paused" : "unpaused")} by {senderId}");

		if(shouldPause)
			SpecialUI("PauseMenu", !playerShouldBePaused[Multiplayer.GetUniqueId()]); // Show pause menu if the local player has to wait for the other player(s) to unpause, otherwise hide it
		else
			SpecialUI("PauseMenu", false);
	}

	private void SpecialUI(string tag, bool value)
	{
		GetTree().GetNodesInGroup(tag).Cast<Control>().ToList().ForEach(group => group.Visible = value);
		GD.Print($"Setting visibility of UI group '{tag}' to {value}");
	}

	private void SetInitialPauseState()
	{
		foreach (var peerId in Multiplayer.GetPeers())
		{
			playerShouldBePaused[peerId] = true; // Assume all players should be paused until they explicitly unpause
		}
	}

	private void OnOfflineModeActivated()
	{
		playerShouldBePaused.Clear(); // Clear all pause states since we're switching to offline mode
		playerShouldBePaused[Multiplayer.GetUniqueId()] = false; // Unpause the local player in offline mode
	}
}
