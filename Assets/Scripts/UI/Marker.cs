using Godot;
using System;
using System.Collections.Generic;

public partial class Marker : RichTextLabel
{
	[Export]int[] scores = new int[2];

	[Export] private string text_format = "[rainbow][outline_size=25][font_size=75] {0} : {1}";

	public override void _Ready()
	{
		UpdateScore();
	}

	private void AddScore(int amount, int player)
	{
		scores[player] += amount;
		UpdateScore();
	}

	private void UpdateScore()
	{
		this.Text = string.Format(text_format, scores[0], scores[1]);
	}
}
