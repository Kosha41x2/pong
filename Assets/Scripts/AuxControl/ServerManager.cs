
using Godot;
using System;
using System.ComponentModel;
using Godot.Collections;
using System.Linq;

public partial class ServerManager : Node
{
	const string IP_ADDRESS = "localhost";
	const int PORT = 42069;

	string ipInput;

	[Export] int max_players = 2;
	ENetMultiplayerPeer peer;
	[Signal] public delegate void ClientConnectedEventHandler();
	[Signal] public delegate void ServerCreatedEventHandler();

	[Signal] public delegate void OfflineModeEventHandler();
	Node paddle1;

	Array<Node> nodesToGiveAuthority = new Array<Node>();

	AdviseText adviseText;

	LineEdit ipField;

	public static ServerManager _instance;

	override public async void _Ready()
	{
		if (_instance != null)
		{
			GD.PrintErr("Multiple instances of ServerManager detected. This should not happen.");
			return;
		}
		_instance = this;

		Multiplayer.PeerConnected += OnPeerConnected;
		Multiplayer.ServerDisconnected += OnServerLost;

		await ToSignal(GetTree(), SceneTree.SignalName.SceneChanged); // Wait for the scene to change before trying to find nodes to give authority to, since they might not be in the tree yet

		paddle1 = GetTree().GetFirstNodeInGroup("ServerPaddles");
		nodesToGiveAuthority = GetTree().GetNodesInGroup("ClientAuthority");
		adviseText = GetTree().GetFirstNodeInGroup("MultiplayerAd") as AdviseText;
	}
	override public void _ExitTree()
	{
		Multiplayer.PeerConnected -= OnPeerConnected;
		Multiplayer.ServerDisconnected -= OnServerLost;
	}
	public void StartServer()
	{
		StartOffline();
		peer = new ENetMultiplayerPeer();
		var result = peer.CreateServer(PORT, max_players);

		if (result != Error.Ok)
		{
			if(adviseText != null)
			{
				adviseText.SetAdviseText($"Failed to start server on port {PORT}. Error: {result}");
				adviseText.ShowAdvise();
				adviseText.FadeOut();
			}
			GD.PrintErr("Failed to create server: " + result);
			return;
		}
		Multiplayer.MultiplayerPeer = peer;

		EmitSignal(nameof(ServerCreated));
		GD.Print("Server started on port " + PORT);
		GD.Print("Local IP Address: " + GetLocalIPAddress());
	}

	public void StartClient()
	{
		StartOffline();
		ipField = GetTree().GetFirstNodeInGroup("IPField") as LineEdit;
		if(ipField == null)
		{
			GD.PrintErr("IP Field not found in the scene. Make sure there is a LineEdit node with the group 'IPField'.");
			return;
		}
		
		peer = new ENetMultiplayerPeer();
		var result = peer.CreateClient(ipField.Text, PORT);

		if (result != Error.Ok)
		{
			if(adviseText != null)
			{
				adviseText.SetAdviseText($"Failed to connect to server at {ipField.Text}:{PORT}. Error: {result}");
				adviseText.ShowAdvise();
				adviseText.FadeOut();
			}
			GD.PrintErr("Failed to create client: " + result);
			return;
		}
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Client connected to " + ipField.Text + ":" + PORT);

		EmitSignal(nameof(ClientConnected));
	}

	public void StartOffline()
	{
		if(OnlineVSOffline._instance != null && OnlineVSOffline._instance.IsOffline)
		{
			if(adviseText != null)
			{
				adviseText.SetAdviseText("Already in offline mode.");
				adviseText.ShowAdvise();
				adviseText.FadeOut();
			}
			GD.Print("Already in offline mode.");
			return;
		}

		peer.Close();
		Multiplayer.MultiplayerPeer = new OfflineMultiplayerPeer();
		ReassignMultiplayerAuthority(Multiplayer.GetUniqueId()); // Reassign authority to the local player since we're going offline
		GD.Print("Started in offline mode.");
		EmitSignal(nameof(OfflineMode));
	}

	private void OnPeerConnected(long id)
    {
        ReassignMultiplayerAuthority(id);
    }

	private void OnServerLost()
	{
		GD.Print("Server lost. Returning to offline mode.");
		StartOffline();
		if(adviseText != null)
		{
			adviseText.SetAdviseText($"Connection lost. Returning to offline mode.");
			adviseText.ShowAdvise();
			adviseText.FadeOut();
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void ReassignMultiplayerAuthority(long id = -1)
	{
		if(!Multiplayer.IsServer())
		{
			GD.PrintErr("Only the server can reassign multiplayer authority.");
			return;
		}

		if(id == -1)
		{
			int[] peerIds = Multiplayer.GetPeers();
			if(peerIds.Length == 0)
			{
				GD.Print("No clients connected. Authority will be assigned to the server.");
				return;
			}
			id = peerIds[0]; // Assign authority to the first connected client
			GD.Print($"Client with ID {id} will be assigned authority.");
		}

		paddle1 = GetTree().GetFirstNodeInGroup("ServerPaddles");
		nodesToGiveAuthority = GetTree().GetNodesInGroup("ClientAuthority");

		if(paddle1 != null && IsInstanceValid(paddle1))
			paddle1.SetMultiplayerAuthority((int)id);

		foreach (Node node in nodesToGiveAuthority)
		{
			if(node != null && IsInstanceValid(node))
				node.SetMultiplayerAuthority((int)id);
		}

		RpcId(id, nameof(AssignAuthorityToClient), id);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
    private void AssignAuthorityToClient(long id)
    {
        paddle1.SetMultiplayerAuthority((int)id);
    }

	public string GetLocalIPAddress()
	{
		var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
		var ipAddress = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
		return ipAddress?.ToString() ?? "No network adapters with an IPv4 address in the system!";
	}

	public string GetConnectedIPAddress()
	{
		if (Multiplayer.IsServer())
		{
			return GetLocalIPAddress();
		}
		else
		{
			var enetPeer = Multiplayer.MultiplayerPeer as ENetMultiplayerPeer;
			return enetPeer?.GetPeer(1)?.GetRemoteAddress() ?? "Unknown";
		}
	}
}
