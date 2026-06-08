using Godot;
using System;
using Godot.Collections;
using System.Reflection;

public partial class AdviseText : RichTextLabel
{
	[Export] Array<string> adviseFormat = new Array<string>();
	[Export] string adviseText = "";

	[Export] float displayDuration = 3f;

	private void UpdateText()
	{
		
		foreach(string format in adviseFormat)
		{
			Text += format;
		}

		Text += adviseText;
	}

	public void SetAdviseText(string text)
	{
		adviseText = text;
		UpdateText();
	}

	public void ShowAdvise()
	{
		Visible = true;
		Modulate = new Color(Modulate.G, Modulate.G, Modulate.B, 1f); // Reset alpha to 1 in case it was faded out before
	}

	public void FadeOut()
	{
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 0f, displayDuration).SetTrans(Tween.TransitionType.Linear);
	}
}
