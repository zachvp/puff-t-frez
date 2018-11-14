using UnityEngine;

public class PlayerInputControllerKeyboard : InputController<PlayerInput, PlayerMarionette>
{
	public PlayerInputControllerKeyboard(ICoreInput<PlayerInput> r, InputBuffer<InputSnapshot<PlayerInput>> b)
		: base(r, b)
	{ }

	protected override void UpdateInput()
	{
        // Direction control
		input.direction.Update(Direction2D.RIGHT, Input.GetKey(KeyCode.D));
        input.direction.Update(Direction2D.LEFT, Input.GetKey(KeyCode.A));
		input.direction.Update(Direction2D.UP, Input.GetKey(KeyCode.W));
		input.direction.Update(Direction2D.DOWN, Input.GetKey(KeyCode.S));

		input.jump = Input.GetKey(KeyCode.W);
		input.crouch = Input.GetKey(KeyCode.S);   
	}
}
