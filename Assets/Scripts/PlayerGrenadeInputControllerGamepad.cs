using InControl;

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

			input.direction = new CoreDirection(device.LeftStick.Value);
			input.launch = device.RightBumper.IsPressed;

			input.direction.CardinalizeVector();
		}
	}
}
