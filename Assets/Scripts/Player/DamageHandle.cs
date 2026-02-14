using Godot;
using System;

public partial class DamageHandle : Node2D, IDamagable
{
	[Signal] public delegate void PlayerDamagedEventHandler(int damage, int playerN);
	[Export] public int playerN { get; set; } = 0;
	public void TakeDamage(int damage)
	{
		GD.Print("Player took " + damage + " damage!");
		EmitSignal(nameof(PlayerDamaged), -damage, playerN);
	}
}
