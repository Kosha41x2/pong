using Godot;
using System;

public partial class ColorController : Node
{
	Sprite2D sprite;

	public override void _Ready()
	{
		sprite = GetParent<Sprite2D>();

		if (sprite == null)
		{
			GD.PrintErr("ColorController must be a child of a Sprite2D node.");
		}
	}

	public void OnColorSelected(Color color)
	{
		if (sprite != null)
		{
			sprite.Modulate = color;
			GD.Print($"Player color changed to: {color}");
		}
	}
}
