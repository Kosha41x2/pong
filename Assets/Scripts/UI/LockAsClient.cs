using Godot;
using System;
using System.Reflection;

public partial class LockAsClient : Control
{
	public override void _Ready()
	{
		ServerManager serverManager = ServerManager._instance;
		if (serverManager != null)
		{
			serverManager.ClientConnected += OnClientPressed;
		}
		else
		{
			GD.PrintErr("ServerManager instance not found. LockAsClient will not function properly.");
		}
	}
	public void OnClientPressed()
	{
		bool isOffline = Multiplayer.MultiplayerPeer == null || Multiplayer.MultiplayerPeer.GetConnectionStatus() == MultiplayerPeer.ConnectionStatus.Disconnected;

		Control parentControl = GetParent<Control>();

		if (!isOffline && !Multiplayer.IsServer())
		{
			parentControl.Modulate = new Color(parentControl.Modulate.R, parentControl.Modulate.G, parentControl.Modulate.B, 0.5f);
			parentControl.MouseFilter = MouseFilterEnum.Ignore;



			if(parentControl is SpinBox spinBox)
			{
				spinBox.Editable = false;
			} else if(parentControl is Button button)
			{
				button.Disabled = true;
			} else if(parentControl is HSlider hSlider)
			{
				hSlider.Editable = false;
			} else if(parentControl is LineEdit lineEdit)
			{
				lineEdit.Editable = false;
			}

			GD.Print("Locked as client.");
		}else
		{
			parentControl.Modulate = new Color(parentControl.Modulate.R, parentControl.Modulate.G, parentControl.Modulate.B, 1f);
			parentControl.MouseFilter = MouseFilterEnum.Stop;

			if(parentControl is SpinBox spinBox)
			{
				spinBox.Editable = true;
			} else if(parentControl is Button button)
			{
				button.Disabled = false;
			} else if(parentControl is HSlider hSlider)
			{
				hSlider.Editable = true;
			} else if(parentControl is LineEdit lineEdit)
			{
				lineEdit.Editable = true;
			}

			GD.Print("Unlocked as client.");
		}
	}
}
