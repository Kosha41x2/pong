using Godot;
using System;

public partial class ServerUI : Node
{
	public void OnServerPressed()
	{
		ServerManager._instance.StartServer(); 
	}

	void OnClientPressed()
	{
		ServerManager._instance.StartClient();
	}
}
