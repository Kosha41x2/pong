using Godot;
using System;

public partial class PlayerData : CharacterBody2D
{
	[Export] public int PlayerN { get; private set; } = 0;
}
