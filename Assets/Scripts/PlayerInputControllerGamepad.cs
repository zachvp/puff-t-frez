using InControl;
using UnityEngine;

public class PlayerInputControllerGamepad : InputController<PlayerInput, PlayerMarionette>
{
	public PlayerInputControllerGamepad(ICoreInput<PlayerInput> r,
	                                    InputBuffer<InputSnapshot<PlayerInput>> b)
		: base(r, b)
	{ }

	public override void HandleUpdate(long currentFrame, float deltaTime)
    {
		base.HandleUpdate(currentFrame, deltaTime);

        if (InputManager.Devices.Count > 0)
		{
            var device = InputManager.Devices[0];
			var crouchThreshold = 0.7f;
			var leftStick = device.LeftStick.Value;

			// TODO: Put in utility class
			var threshold = 0.2f;
			var newDir = input.direction.Vector;

			if (Mathf.Abs(leftStick.x) > threshold)
			{
				newDir.x = leftStick.x;
			}
			if (Mathf.Abs(leftStick.y) > threshold)
			{
				newDir.y = leftStick.y;
			}

			input.direction.Update(newDir);
			input.direction.CardinalizeVector();

			input.crouch = leftStick.y < -crouchThreshold;
			input.jump = device.Action1.IsPressed;
        }
    }
}
