using Godot;
using System;
using Godot.Collections;
using System.ComponentModel;

[Tool]
public partial class Frame : Node
{
	[Export] private PackedScene DisplayObjectScene;
	private Sprite2D _displayObject;

	private Ball ball;

	private TextureRect _textureRect;
	private RichTextLabel _textLabel;

	[Export] Array<string> textFormat = new Array<string>();
	[Export] Array<string> tittleFormat = new Array<string>();
	[Export(PropertyHint.MultilineText)] string text;
	[Export(PropertyHint.MultilineText)] string TittleText;

	public override void _Ready()
	{
		ball = DisplayObjectScene.Instantiate<Ball>();

		_displayObject = ball.GetChild<Sprite2D>(1);

		_textureRect = GetChild<TextureRect>(1);
		
		_textLabel = GetChild<RichTextLabel>(0);

		_textureRect.Texture = _displayObject.Texture;

		_textureRect.Modulate = _displayObject.Modulate;

		_textLabel.Text = "";

		
		foreach (string format in tittleFormat)
		{
			_textLabel.Text += format;
		}
		_textLabel.Text += TittleText + "\n";

		foreach (string format in textFormat)
		{
			_textLabel.Text += format;
		}
		_textLabel.Text += text + "\n\nThis ball gives " + ball.PointsOfValue + " points.\n" + "Initial Speed: " + ball.initialSpeed;
	}
}
