using Godot;
using Godot.Collections;
using System;

public partial class ShowSelectedNode : Node
{
	[Export]int selectedNodeIndex = 0;
	[Export]Array<Control> nodesToShow = new Array<Control>();

	override public void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is Control control)
			{
				nodesToShow.Add(control);
				control.Visible = false; // Hide all nodes initially
			}
			else
			{
				GD.PrintErr($"Child node '{child.Name}' is not a Control. ShowSelectedNode will only work with Control nodes.");
			}
		}

		OnNodeSelected(selectedNodeIndex);
	}
	void OnNodeSelected(int index)
	{
		selectedNodeIndex = index;
		HideAllNodes();
		ShowIndexNode();
		GD.Print($"Selected node index: {selectedNodeIndex}");
	}

	private void ShowIndexNode()
	{
		nodesToShow[selectedNodeIndex].Visible = true;
		nodesToShow[selectedNodeIndex].MouseFilter = Control.MouseFilterEnum.Stop;
	}

	private void HideAllNodes()
	{
		foreach (Control node in nodesToShow)
		{
			node.Visible = false;
			node.MouseFilter = Control.MouseFilterEnum.Ignore;
		}
	}
}
