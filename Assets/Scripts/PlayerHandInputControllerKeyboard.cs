using UnityEngine;

public class PlayerHandInputControllerKeyboard :
    InputController<HandInput, PlayerMarionette>
{
	public PlayerHandInputControllerKeyboard(ICoreInput<HandInput> r,
	                                         InputBuffer<InputSnapshot<HandInput>> b)
		: base (r, b)
	{ }

	public override void HandleUpdate(long currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

		input.direction.Update(Direction2D.RIGHT, Input.GetKey(KeyCode.RightArrow));
        input.direction.Update(Direction2D.LEFT, Input.GetKey(KeyCode.LeftArrow));
		input.direction.Update(Direction2D.UP, Input.GetKey(KeyCode.UpArrow));
		input.direction.Update(Direction2D.DOWN, Input.GetKey(KeyCode.DownArrow));
	}
}
