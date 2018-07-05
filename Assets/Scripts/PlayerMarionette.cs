using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs

public class PlayerMarionette : IPlayerMarionette
{
	private IPlayerInput playerInput;
	private IMotor playerMotor;

	private IBehavior handBehavior;

	private ILobInput handGrenadeInput;
	private IBehavior handGrenadeBehavior;

	private CallbackManager manager;

	private int inputCount;
        
	public PlayerMarionette() {
		manager = new CallbackManager();
	}

	public void AttachBody(IPlayerInput body, IMotor motor) {
		playerInput = body;
		playerMotor = motor;
	}

	public void AttachHand(IBehavior behavior) {
		handBehavior = behavior;
	}

	public void AttachHandGrenade(ILobInput lobInput, IBehavior behavior) {
		handGrenadeInput = lobInput;
		handGrenadeBehavior = behavior;
		handGrenadeInput.Reset();
	}

	// IPlayerMarionette begin
	public void ApplyPlayerInput(PlayerInputSnapshot snapshot)
	{
		playerInput.ApplyInput(snapshot);
	}

	// TODO: Cleanup input available checks
	// TODO: Need to handle direction
	public void ApplyGrenadeInput()
	{
		// TODO: fix magic numbers
		if (inputCount < 4)
		{
			if (inputCount == 0)
            {
				// Handle initial launch
				handBehavior.SetActive(false);
                handGrenadeInput.Reset();
                manager.PostCallbackWithFrameDelay(160, new Callback(HandleResetPosition));
            }

			var mult = 0.9f;

			if (playerMotor.GetDirection().x < 0)
			{
				handGrenadeInput.Lob(Direction2D.LEFT, playerMotor.GetVelocity() * mult);
			}
			else
			{
				handGrenadeInput.Lob(Direction2D.RIGHT, playerMotor.GetVelocity() * mult);
			}

			inputCount++;
		}
	}

	public void ApplyDeltaTime(float deltaTime)
	{
		playerInput.ApplyDeltaTime(deltaTime);
	}
	// IPlayerMarionette end
    
    // TODO: fix magic numbers
    public void HandleResetPosition()
    {
		inputCount = 0;
		handBehavior.SetActive(true);
		handGrenadeInput.Reset();
    }
}

public interface IPlayerMarionette
{
	void ApplyPlayerInput(PlayerInputSnapshot snapshot);
	void ApplyGrenadeInput();

	void ApplyDeltaTime(float deltaTime);
}
