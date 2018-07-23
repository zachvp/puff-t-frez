using UnityEngine;

// TODO: Remove
public class PlayerInputController : InputController<PlayerInput, PlayerMarionette>
{    
	public PlayerInputController(ICoreInput<PlayerInput> r, InputBuffer<InputSnapshot<PlayerInput>> b)
		: base(r, b)
	{}
}
