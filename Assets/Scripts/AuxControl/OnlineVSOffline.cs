using Godot;
using System;

public partial class OnlineVSOffline : Node
{
	static public OnlineVSOffline _instance;
	[Signal] public delegate void OnlineModeActivatedEventHandler();
	[Signal] public delegate void OfflineModeActivatedEventHandler();
	public override void _Ready()
	{
		ServerManager._instance.ClientConnected += SwitchOnlineLogic;
		ServerManager._instance.ServerCreated += SwitchOnlineLogic;
		ServerManager._instance.OfflineMode += SwitchOnlineLogic;
		_instance = this;
	}

	public override void _ExitTree()
	{
		if (ServerManager._instance != null)
		{
			ServerManager._instance.ClientConnected -= SwitchOnlineLogic;
			ServerManager._instance.ServerCreated -= SwitchOnlineLogic;
			ServerManager._instance.OfflineMode -= SwitchOnlineLogic;
		}
	}

	public void SwitchOnlineLogic()
	{
		bool isOffline = Multiplayer.MultiplayerPeer is OfflineMultiplayerPeer || Multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Disconnected;

		if (isOffline)
		{
			EmitSignal(nameof(OfflineModeActivated));
			GD.Print("Running in offline mode.");
		}
		else
		{
			EmitSignal(nameof(OnlineModeActivated));
			GD.Print("Running in online mode.");
		}
	}
}
