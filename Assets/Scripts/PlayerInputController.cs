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

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public override void HandleUpdate(int currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

		HandleUpdate();
	}

	public virtual void HandleUpdate()
	{
		// Subclasses implement this
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
        if (Mathf.Abs(input.movement.x) < 1 && Mathf.Abs(lastInput.movement.x) > 0)
		{
            inputRelease.movement.x = 1;
        }
        if (Mathf.Abs(input.movement.y) < 1 && Mathf.Abs(lastInput.movement.y) > 0)
		{
            inputRelease.movement.y = 1;
        }
		if (lastInput.jump && !input.jump)
		{
            inputRelease.jump = true;
        }
		if (lastInput.crouch && !input.crouch)
		{
			inputRelease.crouch = true;
		}

		var snapshot = new InputSnapshot<PlayerInput>(input, inputRelease);

		player.ApplyPlayerInput(snapshot);
        buffer.AddInput(snapshot);

        Debug.AssertFormat(Mathf.Abs(input.movement.x) <= 1, "Input exceeded bounds: {0}", input);
        Debug.AssertFormat(Mathf.Abs(input.movement.y) <= 1, "Input exceeded bounds: {0}", input);
	}
}
