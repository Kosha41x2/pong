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
	Node paddle1;

	Array<Node> nodesToGiveAuthority = new Array<Node>();

	LineEdit ipField;

	public static ServerManager _instance;

	override public void _Ready()
	{
		if (_instance != null)
		{
			GD.PrintErr("Multiple instances of ServerManager detected. This should not happen.");
			return;
		}
		_instance = this;

		paddle1 = GetTree().GetFirstNodeInGroup("ServerPaddles");
		nodesToGiveAuthority = GetTree().GetNodesInGroup("ClientAuthority");
		ipField = GetTree().GetFirstNodeInGroup("IPField") as LineEdit;

		Multiplayer.PeerConnected += OnPeerConnected;
	}
	public void StartServer()
	{
		peer = new ENetMultiplayerPeer();
		var result = peer.CreateServer(PORT, max_players);
		if (result != Error.Ok)
		{
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
		peer = new ENetMultiplayerPeer();
		var result = peer.CreateClient(ipField.Text, PORT);
		if (result != Error.Ok)
		{
			GD.PrintErr("Failed to create client: " + result);
			return;
		}
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Client connected to " + ipField.Text + ":" + PORT);

		EmitSignal(nameof(ClientConnected));
	}

	private void OnPeerConnected(long id)
    {
        if (Multiplayer.IsServer())
        {
            GD.Print($"Client Connected with: {id}. Giving them to the paddle.");
            paddle1.SetMultiplayerAuthority((int)id);
			foreach (Node node in nodesToGiveAuthority)
			{
				node.SetMultiplayerAuthority((int)id);
			}
            RpcId(id, nameof(AssignAuthorityToClient), id);
        }
    }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void AssignAuthorityToClient(long id)
    {
        paddle1.SetMultiplayerAuthority((int)id);
        GD.Print("I'm the client and I've been assigned authority to the paddle!");
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
