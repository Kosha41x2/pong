using Godot;
using System;
using System.Linq;

public partial class ServerManager : Node
{
	const string IP_ADDRESS = "localhost";
	const int PORT = 42069;

	[Export] int max_players = 2;
	ENetMultiplayerPeer peer;
	[Signal] public delegate void ClientConnectedEventHandler();
	Node paddle1;

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
		GD.Print("Server started on port " + PORT);
	}

	public void StartClient()
	{
		peer = new ENetMultiplayerPeer();
		var result = peer.CreateClient(IP_ADDRESS, PORT);
		if (result != Error.Ok)
		{
			GD.PrintErr("Failed to create client: " + result);
			return;
		}
		Multiplayer.MultiplayerPeer = peer;
		GD.Print("Client connected to " + IP_ADDRESS + ":" + PORT);

		EmitSignal(nameof(ClientConnected));
	}

	private void OnPeerConnected(long id)
    {
        if (Multiplayer.IsServer())
        {
            GD.Print($"Client Connected with: {id}. Giving them to the paddle.");
            paddle1.SetMultiplayerAuthority((int)id);
            RpcId(id, nameof(AssignAuthorityToClient), id);
        }
    }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    private void AssignAuthorityToClient(long id)
    {
        paddle1.SetMultiplayerAuthority((int)id);
        GD.Print("I'm the client and I've been assigned authority to the paddle!");
    }
}
