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

	protected void HandleInputRight()
	{
		FlagsHelper.Set(ref input.direction, Direction2D.RIGHT);
	}

	protected void HandleInputLeft()
	{
		FlagsHelper.Set(ref input.direction, Direction2D.LEFT);
	}

	protected void HandleInputUp()
	{
		FlagsHelper.Set(ref input.direction, Direction2D.UP);
	}

	protected void HandleInputDown()
	{
		FlagsHelper.Set(ref input.direction, Direction2D.DOWN);
	}

	protected void HandleInputJump()
	{
		input.jump = true;
	}

    protected void HandleInputCrouch()
	{
		input.crouch = true;
	}
}
