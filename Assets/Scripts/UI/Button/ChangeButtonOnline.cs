using Godot;
using System;
using System.ComponentModel;

public partial class ChangeButtonOnline : Button
{
	string offlineText = "Server";
	string onlineText;

	[Export] Color onlineColor = new Color(1f, 1f, 1f, 0.5f);
	[Export] Color offlineColor = new Color(1f, 1f, 1f, 1f);
	public override void _Ready()
	{
		OnlineVSOffline._instance.OnlineModeActivated += SetOnlineText;
		OnlineVSOffline._instance.OfflineModeActivated += SetOfflineText;
	}

	public override void _ExitTree()
	{
		if (OnlineVSOffline._instance != null)
		{
			OnlineVSOffline._instance.OnlineModeActivated -= SetOnlineText;
			OnlineVSOffline._instance.OfflineModeActivated -= SetOfflineText;
		}
	}

	public void SetOnlineText()
	{
		if(Multiplayer.IsServer())
		{
			onlineText = "Server (created in " + ServerManager._instance.GetLocalIPAddress() + ")";
		}
		else
		{
			onlineText = "(connected as client to " + ServerManager._instance.GetLocalIPAddress() + ")";
		}
		Text = onlineText;
		Modulate = onlineColor;
	}

	public void SetOfflineText()
	{
		Text = offlineText;
		Modulate = offlineColor;
	}

}
