using UnityEngine;

public class PlayerInputControllerKeyboard : PlayerInputController
{
	public PlayerInputControllerKeyboard(IPlayerMarionette playerInput, InputBuffer<InputSnapshot<PlayerInput>> inputBuffer)
		: base(playerInput, inputBuffer)
	{ }

	public override void HandleUpdate(long currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

		// Horizontal control
		FlagsHelper.Set(ref input.direction,
		                Direction2D.RIGHT,
		                Input.GetKey(KeyCode.RightArrow));
		FlagsHelper.Set(ref input.direction,
						Direction2D.LEFT,
		                Input.GetKey(KeyCode.LeftArrow));
		FlagsHelper.Set(ref input.direction,
						Direction2D.UP,
						Input.GetKey(KeyCode.UpArrow));
		FlagsHelper.Set(ref input.direction,
						Direction2D.DOWN,
						Input.GetKey(KeyCode.DownArrow));

		input.jump = Input.GetKey(KeyCode.UpArrow);
		input.crouch = Input.GetKey(KeyCode.DownArrow);
	}
}
