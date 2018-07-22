using System;
using InControl;
using UnityEngine;

public class PlayerGrenadeInputControllerGamepad : InputController<HandGrenadeInput, IPlayerMarionette>
{
	public PlayerGrenadeInputControllerGamepad(IPlayerMarionette m)
		: base(m)
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
            
			var snapshot = new InputSnapshot<HandGrenadeInput>(oldInput, input);

			responder.ApplyGrenadeInput(snapshot);
		}
	}
}
