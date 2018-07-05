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
	private Entity handGrenadeEntity;
	private bool isHandCollided;

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

	public void AttachHandGrenade(ILobInput lobInput, Entity entity) {
		handGrenadeInput = lobInput;
		handGrenadeEntity = entity;
		handGrenadeInput.Reset();

		handGrenadeEntity.OnTriggerEnter += HandleTriggerEnterHandGrenade;
	}

	// IPlayerMarionette begin
	public void ApplyPlayerInput(PlayerInputSnapshot snapshot)
	{
		playerInput.ApplyInput(snapshot);
	}

	// TODO: Cleanup input available checks
	public void ApplyGrenadeInput()
	{
		// TODO: fix magic numbers
		if (inputCount < 4 && !isHandCollided)
		{
			if (inputCount == 0)
            {
				// Handle initial launch
				handBehavior.SetActive(false);
                handGrenadeInput.Reset();
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
    
    // Handlers begin
    public void HandleResetPosition()
    {
		inputCount = 0;
		isHandCollided = false;
		handBehavior.SetActive(true);
		handGrenadeInput.Reset();
    }

	public void HandleTriggerEnterHandGrenade(Collider2D collider)
    {
        var layer = collider.gameObject.layer;

        // TODO: Define layer values as constants
        if (layer == LayerMask.NameToLayer("Obstacle"))
        {
			isHandCollided = true;
			handGrenadeInput.Freeze();
			manager.PostCallbackWithFrameDelay(64, new Callback(HandleResetPosition));
        }
    }
    // Handlers end
}

public interface IPlayerMarionette
{
	void ApplyPlayerInput(PlayerInputSnapshot snapshot);
	void ApplyGrenadeInput();

	void ApplyDeltaTime(float deltaTime);
}
