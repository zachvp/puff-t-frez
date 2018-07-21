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
		input.direction.x = 1;
	}

	protected void HandleInputLeft()
	{
		input.direction.x = -1;
	}

	protected void HandleInputUp()
	{
		input.direction.y = 1;
	}

	protected void HandleInputDown()
	{
		input.direction.y = -1;
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
			input.direction.x = 0;
		}
        if (isConcurrentVerticalInput)
		{
			input.direction.y = 0;
		}

		// Check for releases.
		// TODO: Move this funcitonaliy to InputSnapshot class
		inputRelease.direction = CoreUtilities.GetInputReleased(lastInput.direction, input.direction);
		inputRelease.jump = CoreUtilities.GetInputReleased(lastInput.jump, input.jump);
		inputRelease.crouch = CoreUtilities.GetInputReleased(lastInput.crouch, input.crouch);

		var snapshot = new InputSnapshot<PlayerInput>(input, inputRelease);

		player.ApplyPlayerInput(snapshot);
        buffer.AddInput(snapshot);

        Debug.AssertFormat(Mathf.Abs(input.direction.x) <= 1, "Input exceeded bounds: {0}", input);
        Debug.AssertFormat(Mathf.Abs(input.direction.y) <= 1, "Input exceeded bounds: {0}", input);
	}
}
