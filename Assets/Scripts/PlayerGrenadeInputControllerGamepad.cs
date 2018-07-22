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

            // TODO Centralize this check
			if (device.LeftStick.X > Constants.Input.DEAD_ZONE)
			{
				FlagsHelper.Set(ref input.direction, Direction2D.RIGHT);
			}
			if (device.LeftStick.X < -Constants.Input.DEAD_ZONE)
			{
				FlagsHelper.Set(ref input.direction, Direction2D.LEFT);
			}

			input.launch = device.RightBumper.IsPressed;

			Debug.AssertFormat(!(FlagsHelper.IsSet(input.direction, Direction2D.LEFT) &&
                     FlagsHelper.IsSet(input.direction, Direction2D.RIGHT)),
                   "Invalid direction given: {0}", input.direction);

			var snapshot = new InputSnapshot<HandGrenadeInput>(oldInput, input);

			responder.ApplyGrenadeInput(snapshot);
		}
	}
}
