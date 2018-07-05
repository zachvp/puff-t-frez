using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs

public class PlayerMarionette : IPlayerMarionette
{
	private IPlayerInput playerInput;
	private IMotor playerMotor;
	private ILobInput handGrenadeInput;
	private CallbackManager manager;

	private bool isInputAvailable;
	private int inputCount;
        
	public PlayerMarionette() {
		manager = new CallbackManager();
		isInputAvailable = true;
	}

	public void AttachBody(IPlayerInput body, IMotor motor) {
		playerInput = body;
		playerMotor = motor;
	}

	public void AttachHandGrenade(ILobInput lobInput) {
		handGrenadeInput = lobInput;
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
		if (isInputAvailable)
		{
			isInputAvailable = false;
			// TODO: fix magic numbers
            
			manager.PostCallbackWithFrameDelay(36, new Callback(HandleCallbackFired));
            
			if (playerMotor.GetDirection().x < 0)
			{
				handGrenadeInput.Lob(Direction2D.LEFT);
			}
			else
			{
				handGrenadeInput.Lob(Direction2D.RIGHT);
			}

			if (inputCount == 0)
            {
                manager.PostCallbackWithFrameDelay(256, new Callback(HandleResetPosition));
            }

			inputCount++;
		}
	}

	public void ApplyDeltaTime(float deltaTime)
	{
		playerInput.ApplyDeltaTime(deltaTime);
	}
	// IPlayerMarionette end

    // CallbackManager handlers
	public void HandleCallbackFired()
    {
        isInputAvailable = true;
    }
    // TODO: fix magic numbers
    public void HandleResetPosition()
    {
		isInputAvailable = true;
		inputCount = 0;
		//handGrenadeInput.Freeze();
		handGrenadeInput.Reset();
    }
}

public interface IPlayerMarionette
{
	void ApplyPlayerInput(PlayerInputSnapshot snapshot);
	void ApplyGrenadeInput();

	void ApplyDeltaTime(float deltaTime);
}
