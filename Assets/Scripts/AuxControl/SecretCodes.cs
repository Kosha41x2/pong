using Godot;
using System;
using Godot.Collections;

public partial class SecretCodes : Node
{
	public static SecretCodes Instance {get; private set;}
	private string inputBuffer = "";

	[Signal] public delegate void SecretCodeEnteredEventHandler(string code);

	[Export] private int maxBufferSize = 10;
	[Export] public Dictionary<string, bool> secretCodes {get; private set;} = new Dictionary<string, bool>();

	public override void _Ready()
	{
		Instance = this;
		
		ResetSecretCodes();
	}

	private void ResetSecretCodes()
	{
		foreach (string code in secretCodes.Keys)
		{
			secretCodes[code] = false;
		}
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        if(@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.IsEcho())
		{
			if(keyEvent.Unicode == 0) return; // Non-character key, ignore

			inputBuffer += char.ConvertFromUtf32((char)keyEvent.Unicode).ToLower();

			if(inputBuffer.Length > maxBufferSize)
			{
				inputBuffer = inputBuffer.Substring(1);
			}

			foreach (string code in secretCodes.Keys)
			{
				if (inputBuffer.EndsWith(code))
				{
					ResetSecretCodes();
					secretCodes[code] = !secretCodes[code];
					EmitSignal("SecretCodeEntered", code);
					inputBuffer = "";
					break;
				}
			}
		}
    }
}
