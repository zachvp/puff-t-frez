using UnityEngine;

public class PlayerInputControllerKeyboard : PlayerInputController
{
	public PlayerInputControllerKeyboard(ICoreInput<PlayerInput> r, InputBuffer<InputSnapshot<PlayerInput>> b)
		: base(r, b)
	{ }

	public override void HandleUpdate(long currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

        // Direction control
		input.direction.Update(Direction2D.RIGHT, Input.GetKey(KeyCode.RightArrow));
        input.direction.Update(Direction2D.LEFT, Input.GetKey(KeyCode.LeftArrow));
		input.direction.Update(Direction2D.UP, Input.GetKey(KeyCode.UpArrow));
		input.direction.Update(Direction2D.DOWN, Input.GetKey(KeyCode.DownArrow));

		input.jump = Input.GetKey(KeyCode.UpArrow);
		input.crouch = Input.GetKey(KeyCode.DownArrow);
	}
}
