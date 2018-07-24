using UnityEngine;

// TODO: This isn't tied into replay - need to buffer input at the marionette level i think
public class PlayerGrenadeInputControllerKeyboard : InputController<HandGrenadeInput, PlayerMarionette>
{
	public PlayerGrenadeInputControllerKeyboard(ICoreInput<HandGrenadeInput> r,
	                                            InputBuffer<InputSnapshot<HandGrenadeInput>> b)
		: base(r, b)
	{ }

	public override void HandleUpdate(long currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

		input.direction.Update(Direction2D.RIGHT, Input.GetKey(KeyCode.RightArrow));
		input.direction.Update(Direction2D.LEFT, Input.GetKey(KeyCode.LeftArrow));
        
		input.launch = Input.GetKey(KeyCode.D);
	}
}
