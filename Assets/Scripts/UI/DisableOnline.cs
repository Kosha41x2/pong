using Godot;
using System;

public partial class DisableOnline : Control
{
	Control parentNode;

	public override void _Ready()
	{
		parentNode = GetParent() as Control;
		ServerManager._instance.ClientConnected += OnOnlinePressed;
		ServerManager._instance.ServerCreated += OnOnlinePressed;
		ServerManager._instance.OfflineMode += OnOfflinePressed;
	}

	public override void _ExitTree()
	{
		if (ServerManager._instance != null)
		{
			ServerManager._instance.ClientConnected -= OnOnlinePressed;
			ServerManager._instance.ServerCreated -= OnOnlinePressed;
			ServerManager._instance.OfflineMode -= OnOfflinePressed;
		}
	}
	public void OnOnlinePressed()
	{
		if(parentNode != null)
		{
			parentNode.Modulate = new Color(parentNode.Modulate.R, parentNode.Modulate.G, parentNode.Modulate.B, 0.5f);
			parentNode.MouseFilter = MouseFilterEnum.Ignore;

			if(parentNode is OptionButton optionButton)
			{
				optionButton.Disabled = true;

				if(Multiplayer.IsServer())
				{
					optionButton.Selected = 0; // Set to "Offline" option
				}
				else
				{
					optionButton.Selected = 1; // Set to "Client" option
				}
				optionButton.EmitSignal("item_selected", optionButton.Selected);
			}
			if(parentNode is Button button)
			{
				GD.Print("Disabling button for online mode.");
				button.MouseFilter = MouseFilterEnum.Ignore;
				button.Modulate = new Color(button.Modulate.R, button.Modulate.G, button.Modulate.B, 0.5f);
				button.Disabled = true;
			}

		} else
		{
			GD.PrintErr("Parent node is not a Control. DisableOnline will not function properly.");
		}
	}
	public void OnOfflinePressed()
	{
		if(parentNode != null)
		{
			parentNode.Modulate = new Color(parentNode.Modulate.R, parentNode.Modulate.G, parentNode.Modulate.B, 1f);
			parentNode.MouseFilter = MouseFilterEnum.Stop;

			if(parentNode is OptionButton optionButton)
			{
				optionButton.Disabled = false;
			}

			if(parentNode is Button button)
			{
				button.Disabled = false;
				button.MouseFilter = MouseFilterEnum.Stop;
				button.Modulate = new Color(button.Modulate.R, button.Modulate.G, button.Modulate.B, 1f);
			}
		}
		else
		{
			GD.PrintErr("Parent node is not a Control. DisableOnline will not function properly.");
		}
	}
}
