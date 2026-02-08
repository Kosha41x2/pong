using Godot;
using System;
using System.Threading.Tasks.Dataflow;

public partial class Hola : Node
{
	public override void _Ready()
	{
		GD.Print("Hola Mundo");
	}
	public override void _Process(double delta)
	{
	}
}
