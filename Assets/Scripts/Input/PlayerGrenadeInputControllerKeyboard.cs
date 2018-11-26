using UnityEngine;

// TODO: This isn't tied into replay - need to buffer input at the marionette level i think
public class PlayerGrenadeInputControllerKeyboard : InputController<HandGrenadeInput, PlayerMarionette>
{
	public PlayerGrenadeInputControllerKeyboard(ICoreInput<HandGrenadeInput> r,
	                                            InputBuffer<InputSnapshot<HandGrenadeInput>> b)
		: base(r, b)
	{ }

    protected override void UpdateInput()
    {
        input.direction.Update(Direction2D.RIGHT, Input.GetKey(KeyCode.D));
        input.direction.Update(Direction2D.LEFT, Input.GetKey(KeyCode.A));

        input.launch = Input.GetKey(KeyCode.E);

		input.direction.ClearConcurrent();
    }
}
