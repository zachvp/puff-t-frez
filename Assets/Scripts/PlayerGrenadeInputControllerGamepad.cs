using System;
using InControl;
using UnityEngine;

public class PlayerGrenadeInputControllerGamepad : InputController<HandGrenadeInput, PlayerMarionette>
{
	public PlayerGrenadeInputControllerGamepad(ICoreInput<HandGrenadeInput> r, InputBuffer<InputSnapshot<HandGrenadeInput>> b)
		: base(r, b)
	{ }

	public override void HandleUpdate(long currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

		if (InputManager.Devices.Count > 0)
		{
			var device = InputManager.Devices[0];
			var leftStick = device.LeftStick.Value;

			input.direction = CoreUtilities.Convert(leftStick);
			input.launch = device.RightBumper.IsPressed;
		}
	}
}
