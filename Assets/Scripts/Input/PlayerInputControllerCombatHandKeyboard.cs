using UnityEngine;

public class PlayerInputControllerCombatHand : InputController<CombatHandInput, PlayerMarionette>
{
	public PlayerInputControllerCombatHand(ICoreInput<CombatHandInput> r, InputBuffer<InputSnapshot<CombatHandInput>> b)
        : base(r, b)
    {
		
    }

	protected override void UpdateInput()
	{
		// Direction control
        input.direction.Update(Direction2D.RIGHT, Input.GetKey(KeyCode.D));
        input.direction.Update(Direction2D.LEFT, Input.GetKey(KeyCode.A));
        input.direction.Update(Direction2D.UP, Input.GetKey(KeyCode.W));
        input.direction.Update(Direction2D.DOWN, Input.GetKey(KeyCode.S));

		input.grab = Input.GetKey(KeyCode.R);
	}
}
