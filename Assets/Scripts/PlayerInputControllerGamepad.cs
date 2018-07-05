using InControl;

public class PlayerInputControllerGamepad : PlayerInputController
{
	public override void HandleUpdate()
    {
        if (InputManager.Devices.Count > 0) {
            var device = InputManager.Devices[0];

            if (device.LeftStick.Value.x > 0.1f) {
                HandleInputRight();
            }
            if (device.LeftStick.Value.x < -0.1f) {
                HandleInputLeft();
            }
            if (device.LeftStick.Value.y > 0.1f) {
                HandleInputUp();
            }
            if (device.LeftStick.Value.y < -0.1f) {
                HandleInputDown();
            }

            if (device.Action1.IsPressed) {
                HandleInputJump();
            }
        }

        HandleInputChecksFinished(false, false);
    }
}
