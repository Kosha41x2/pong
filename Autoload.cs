using Godot;
using System;

public partial class Autoload : Node
{
	public static Autoload Instance {get; private set;}

	public override void _Ready()
	{
		Instance = this;
	}

	
}
