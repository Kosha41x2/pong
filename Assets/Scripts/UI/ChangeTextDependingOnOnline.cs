using Godot;
using System;

public partial class ChangeTextDependingOnOnline : RichTextLabel
{
	[Export(PropertyHint.MultilineText)] string offlineText;
	[Export(PropertyHint.MultilineText)] string onlineText;
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
		Text = onlineText;
		GD.Print("Text set to online version.");
	}

	public void SetOfflineText()
	{
		Text = offlineText;
		GD.Print("Text set to offline version.");
	}
}
