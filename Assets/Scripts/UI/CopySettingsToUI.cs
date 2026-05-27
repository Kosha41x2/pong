using Godot;
using System;

public partial class CopySettingsToUI : Control
{
	private enum SettingType
	{
		MaxSpeed,
		SpeedIncreaseFactor,
		BounceMaxAngle,
		VisibleControls
	}

	[Export] private SettingType settingType;

	public override void _Ready()
	{
		CopySettings();
	}
	void CopySettings()
	{
		switch (settingType)
		{
			case SettingType.MaxSpeed:
				if(GetParent() is SpinBox spinBox)
				{
					spinBox.Value = Settings.Instance.maxSpeed;
				}
				break;
			case SettingType.SpeedIncreaseFactor:
				if(GetParent() is SpinBox spinBox2)
				{
					spinBox2.Value = (Settings.Instance.speedIncreaseFactor - 1f) * 100f;
				}
				break;
			case SettingType.BounceMaxAngle:
				if(GetParent() is SpinBox spinBox3)
				{
					spinBox3.Value = Settings.Instance.bounceMaxAngle;
				}
				break;
			case SettingType.VisibleControls:
				if(GetParent() is BaseButton toggleButton)
				{
					toggleButton.ButtonPressed = Settings.Instance.visibleControls;
				}
				break;
		}
	}
}
