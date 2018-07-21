using InControl;

public class PlayerInputControllerGamepad : PlayerInputController
{
	public override void HandleUpdate(int currentFrame, float deltaTime)
    {
		base.HandleUpdate(currentFrame, deltaTime);

        if (InputManager.Devices.Count > 0)
		{
            var device = InputManager.Devices[0];

			if (device.LeftStick.Value.x > Constants.Input.DEAD_ZONE)
			{
                HandleInputRight();
            }
			if (device.LeftStick.Value.x < -Constants.Input.DEAD_ZONE)
			{
                HandleInputLeft();
            }
			if (device.LeftStick.Value.y > Constants.Input.DEAD_ZONE)
			{
                HandleInputUp();
            }
			if (device.LeftStick.Value.y < -Constants.Input.DEAD_ZONE)
			{
                HandleInputDown();
            }

            if (device.Action1.IsPressed)
			{
                HandleInputJump();
            }
        }

        HandleInputChecksFinished(false, false);
    }
}
