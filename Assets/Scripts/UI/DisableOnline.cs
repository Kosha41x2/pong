using Godot;
using System;

public partial class DisableOnline : Control
{
	Control parentNode;

	public override void _Ready()
	{
		parentNode = GetParent() as Control;
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
		}
		else
		{
			GD.PrintErr("Parent node is not a Control. DisableOnline will not function properly.");
		}
	}
}
