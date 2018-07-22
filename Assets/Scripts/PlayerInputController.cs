using UnityEngine;

public class PlayerInputController : InputController<PlayerInput, IPlayerMarionette>
{    
	public PlayerInputController(IPlayerMarionette m, 
	                             InputBuffer<InputSnapshot<PlayerInput>> inputBuffer)
		: base(m)
	{
		buffer = inputBuffer;
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

	protected void HandleInputChecksFinished(bool isConcurrentHorizontalInput, bool isConcurrentVerticalInput)
	{
		if (isConcurrentHorizontalInput)
		{
			FlagsHelper.Unset(ref input.direction, Direction2D.RIGHT | Direction2D.LEFT);
		}
        if (isConcurrentVerticalInput)
		{
			FlagsHelper.Unset(ref input.direction, Direction2D.UP | Direction2D.DOWN);
		}

		// TODO: Refactor so pass (held: input, oldInput : oldInput, pressed: )
		var snapshot = new InputSnapshot<PlayerInput>(oldInput, input);
        
		responder.ApplyPlayerInput(snapshot);
        buffer.AddInput(snapshot);
	}
}
