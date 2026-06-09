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
            // 1. Get the raw file paths of the textures to see what Godot ACTUALLY loaded
            string currentPath = sprite.Texture != null ? sprite.Texture.ResourcePath : "No Texture";
            string targetPath = secretSprites[code].ResourcePath;

            // Print exactly what Godot sees
            GD.Print($"[DEBUG] Code typed: {code}");
            GD.Print($"[DEBUG] Current Sprite Path: {currentPath}");
            GD.Print($"[DEBUG] Target Sprite Path: {targetPath}");

            // 2. Compare the file paths, NOT the C# objects
            if(currentPath == targetPath)
            {
                if(secretSprites.ContainsKey(defaultCode) && secretSprites[defaultCode] != null)
                {
                    GD.Print("[DEBUG] Paths matched! Reverting to default sprite.");
                    sprite.Texture = secretSprites[defaultCode];
                }
                return;
            }

            GD.Print($"[DEBUG] Paths differ. Changing sprite to: {targetPath}");
            sprite.Texture = secretSprites[code];
        }
        else
        {
            GD.Print($"[DEBUG] Code '{code}' not found in dictionary, or sprite/texture is null.");
        }
    }
}
