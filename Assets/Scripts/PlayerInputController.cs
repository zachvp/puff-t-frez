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
		input.movement.x = 1;
	}

	protected void HandleInputLeft()
	{
		input.movement.x = -1;
	}

	protected void HandleInputUp()
	{
		input.movement.y = 1;
	}

	protected void HandleInputDown()
	{
		input.movement.y = -1;
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
		var isNoHorizontalInput = Mathf.RoundToInt(input.movement.x) == 0;
		var isNoVerticalInput = Mathf.RoundToInt(input.movement.y) == 0;

		if (isNoHorizontalInput || isConcurrentHorizontalInput)
		{
            input.movement.x = 0;
		}
        if (isNoVerticalInput || isConcurrentVerticalInput)
		{
            input.movement.y = 0;
		}

		// Check for releases.
		// TODO: Move this funcitonaliy to InputSnapshot class
		inputRelease.movement = CoreUtilities.GetInputReleased(lastInput.movement, input.movement);
		inputRelease.jump = CoreUtilities.GetInputReleased(lastInput.jump, input.jump);
		inputRelease.crouch = CoreUtilities.GetInputReleased(lastInput.crouch, input.crouch);

		var snapshot = new InputSnapshot<PlayerInput>(input, inputRelease);

		player.ApplyPlayerInput(snapshot);
        buffer.AddInput(snapshot);

        Debug.AssertFormat(Mathf.Abs(input.movement.x) <= 1, "Input exceeded bounds: {0}", input);
        Debug.AssertFormat(Mathf.Abs(input.movement.y) <= 1, "Input exceeded bounds: {0}", input);
	}
}
