using System;
using InControl;
using UnityEngine;

public class PlayerGrenadeInputControllerGamepad : InputController<HandGrenadeInput>
{
	private IPlayerMarionette marionette;

	public PlayerGrenadeInputControllerGamepad(IPlayerMarionette m)
	{
		marionette = m;
	}

	public override void HandleUpdate(long currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

		if (InputManager.Devices.Count > 0)
		{
			var device = InputManager.Devices[0];

			if (device.LeftStick.X > Constants.Input.DEAD_ZONE)
			{
				FlagsHelper.Set(ref input.direction, Direction2D.RIGHT);
			}
			if (device.LeftStick.X < -Constants.Input.DEAD_ZONE)
			{
				FlagsHelper.Set(ref input.direction, Direction2D.RIGHT);
			}

			input.launch = device.RightBumper.IsPressed;

			Debug.AssertFormat(!(FlagsHelper.IsSet(input.direction, Direction2D.LEFT) &&
                     FlagsHelper.IsSet(input.direction, Direction2D.RIGHT)),
                   "Invalid direction given: {0}", input.direction);

			var snapshot = new InputSnapshot<HandGrenadeInput>(input, oldInput);

            marionette.ApplyGrenadeInput(snapshot);
		}
	}
}
