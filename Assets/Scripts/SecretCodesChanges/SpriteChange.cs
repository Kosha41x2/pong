using Godot;
using System;
using Godot.Collections;

public partial class SpriteChange : Node
{
	Sprite2D sprite;

	private const string defaultCode = "default";

	[Export] private Dictionary<string, Texture2D> secretSprites = new Dictionary<string, Texture2D>();

	public override void _Ready()
	{
		sprite = GetParent<Sprite2D>();
		if(SecretCodes.Instance != null)
		{
			SecretCodes.Instance.SecretCodeEntered += OnSecretCodeEntered;
		}
	}

	public override void _ExitTree()
	{
		if(SecretCodes.Instance != null)
		{
			SecretCodes.Instance.SecretCodeEntered -= OnSecretCodeEntered;
		}
	}

private void OnSecretCodeEntered(string code)
    {
        if(secretSprites.ContainsKey(code) && secretSprites[code] != null && sprite != null)
        {
            string currentPath = sprite.Texture != null ? sprite.Texture.ResourcePath : "No Texture";
            string targetPath = secretSprites[code].ResourcePath;

            if(currentPath == targetPath)
            {
                if(secretSprites.ContainsKey(defaultCode) && secretSprites[defaultCode] != null)
                {
                    sprite.Texture = secretSprites[defaultCode];
                }
                return;
            }
            sprite.Texture = secretSprites[code];
        }
        else
        {
            GD.Print($"[DEBUG] Code '{code}' not found in dictionary, or sprite/texture is null.");
        }
    }
}
