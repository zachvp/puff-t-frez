using UnityEngine;

public class PlayerInputController : InputController<PlayerInput>
{
	protected IPlayerMarionette player;
    
	public PlayerInputController() { }

	public PlayerInputController(IPlayerMarionette inPlayer, 
	                             InputBuffer<InputSnapshot<PlayerInput>> inputBuffer)
	{
		player = inPlayer;
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
		FlagsHelper.Set(ref input.direction, Direction2D.ABOVE);
	}

	protected void HandleInputDown()
	{
		FlagsHelper.Set(ref input.direction, Direction2D.BELOW);
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
			FlagsHelper.Unset(ref input.direction, Direction2D.ABOVE | Direction2D.BELOW);
		}

		// Check for releases.
		// TODO: Move this funcitonaliy to InputSnapshot class
		inputRelease.direction = CoreUtilities.GetInputReleased(lastInput.direction, input.direction);
		inputRelease.jump = CoreUtilities.GetInputReleased(lastInput.jump, input.jump);
		inputRelease.crouch = CoreUtilities.GetInputReleased(lastInput.crouch, input.crouch);

		var snapshot = new InputSnapshot<PlayerInput>(input, inputRelease);

		player.ApplyPlayerInput(snapshot);
        buffer.AddInput(snapshot);
	}
}
