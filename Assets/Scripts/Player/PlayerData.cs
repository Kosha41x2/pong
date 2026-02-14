using Godot;
using System;

public partial class PlayerData : CharacterBody2D
{
	[Export] public int PlayerN { get; private set; } = 0;

	public override void _Ready()
	{
		PlayerN = GetParent<PlayerData>().PlayerN;
	}
}
