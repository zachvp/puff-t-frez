using InControl;

public class PlayerInputControllerGamepad : PlayerInputController
{
	public PlayerInputControllerGamepad(IPlayerMarionette inPlayer,
                             InputBuffer<InputSnapshot<PlayerInput>> inputBuffer)
		: base(inPlayer, inputBuffer)
	{ }

	public override void HandleUpdate(long currentFrame, float deltaTime)
    {
		base.HandleUpdate(currentFrame, deltaTime);

        if (InputManager.Devices.Count > 0)
		{
            var device = InputManager.Devices[0];
			var crouchThreshold = 0.7f;
			var leftStick = device.LeftStick.Value;

			input.direction = CoreUtilities.Convert(leftStick);
			input.crouch = leftStick.y < -crouchThreshold;
			input.jump = device.Action1.IsPressed;
        }
    }
}
