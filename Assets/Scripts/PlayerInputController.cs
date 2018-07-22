using UnityEngine;

public class PlayerInputController : InputController<PlayerInput, IPlayerMarionette>
{    
	public PlayerInputController(IPlayerMarionette m, 
	                             InputBuffer<InputSnapshot<PlayerInput>> inputBuffer)
		: base(m)
	{
		buffer = inputBuffer;
	}

	public override void HandleLateUpdate()
	{
		base.HandleLateUpdate();

		// Check if input directions should be neutralized;
		CoreUtilities.ClearConcurrent(ref input.direction,
                                      Direction2D.HORIZONTAL | Direction2D.VERTICAL);

        var snapshot = new InputSnapshot<PlayerInput>(oldInput, input);

        responder.ApplyPlayerInput(snapshot);
        buffer.AddInput(snapshot);
	}
}
