using Godot;
using System;

public partial class SettingsProxy : Node
{
	public void OnMaxSpeedChange(float value)
	{
		Settings.Instance.OnMaxSpeedChange(value);
	}

	public void OnMaxAngleChange(float value)
	{
		Settings.Instance.OnMaxAngleChange(value);
	}

	public void OnSpeedIncreaseFactorChange(float value)
	{
		Settings.Instance.OnSpeedIncreaseFactorChange(value);
	}

	public void OnVisibleControlsToggled(bool value)
	{
		Settings.Instance.OnVisibleControlsToggled(value);
	}
}
