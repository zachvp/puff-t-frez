using InControl;

public class PlayerGamepadInputController : PlayerInputController
{
	public override void HandleUpdate()
    {
        if (InputManager.Devices.Count > 0) {
            var device = InputManager.Devices[0];

            if (device.Action1.IsPressed) {
                HandleInputUp();
                HandleInputJump();
            }
        }

        HandleInputChecksFinished();
    }
}
