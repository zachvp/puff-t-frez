using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
public class PlayerMarionette : IPlayerMarionette
{
	private IInputPlayerBody bodyInput;
	private IMotor bodyMotor;
	private Entity bodyEntity;

	private CharacterController2D bodyEngine;

	private Entity bodyCrouchEntity;

	private IBehavior handBehavior;

	private IInputLob handGrenadeInput;
	private Entity handGrenadeEntity;
	private bool isHandCollided;

	private CallbackManager manager;
	private int inputCount;
	private PlayerMarionetteData data;
        
	public PlayerMarionette()
	{
		manager = new CallbackManager();
		data = ScriptableObject.CreateInstance<PlayerMarionetteData>();
	}

	public void AttachBody(IInputPlayerBody body,
	                       IMotor motor,
	                       Entity entity,
	                       CharacterController2D engine)
	{
		bodyInput = body;
		bodyMotor = motor;
		bodyEntity = entity;
		bodyEngine = engine;
	}

	public void AttachBodyCrouch(Entity entity)
	{
		bodyCrouchEntity = entity;
		bodyCrouchEntity.SetActive(false);
	}

	public void AttachHand(IBehavior behavior)
	{
		handBehavior = behavior;
	}

	public void AttachHandGrenade(IInputLob lobInput, Entity entity)
	{
		handGrenadeInput = lobInput;
		handGrenadeEntity = entity;
		handGrenadeInput.Reset();

		handGrenadeEntity.OnTriggerEnter += HandleTriggerEnterHandGrenade;
	}

	// IPlayerMarionette begin
	public void ApplyPlayerInput(PlayerInputSnapshot snapshot)
	{
		// TODO: ZVP FIXXX
		if (snapshot.pressed.crouch)
		{
			var crouchPosition = bodyEntity.position;

			crouchPosition.y -= bodyEntity.localScale.y;
            
			bodyEntity.SetActive(false);

			bodyEngine.SetDependencies(bodyCrouchEntity, bodyCrouchEntity.GetComponent<BoxCollider2D>(), bodyCrouchEntity.GetComponent<Rigidbody2D>());

			bodyCrouchEntity.SetActive(true);
			bodyCrouchEntity.SetPosition(crouchPosition);
		}
		else 
		{
			
		}

		bodyInput.ApplyInput(snapshot);

		// TODO: Need to check for obstacle above crouch - use crouch motor
		if (snapshot.released.crouch)
		{
			bodyEngine.SetDependencies(bodyEntity, bodyEntity.GetComponent<BoxCollider2D>(), bodyEntity.GetComponent<Rigidbody2D>());

			bodyEntity.SetActive(true);
			bodyCrouchEntity.SetActive(false);
		}
	}

	public void ApplyGrenadeInput()
	{
		if (isGrenadeInputAvailable())
		{
			if (inputCount == 0)
            {
				// Handle initial launch
				handBehavior.SetActive(false);
                handGrenadeInput.Reset();
            }

			var addVelocity = bodyMotor.GetVelocity() * data.lobVelocityCoefficient;

			if (bodyMotor.GetDirection().x < 0)
			{
				handGrenadeInput.Lob(Direction2D.LEFT, addVelocity);
			}
			else
			{
				handGrenadeInput.Lob(Direction2D.RIGHT, addVelocity);
			}

			inputCount++;
		}
	}

	public void ApplyDeltaTime(float deltaTime)
	{
		bodyInput.ApplyDeltaTime(deltaTime);
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

		if (layer == Constants.Layers.OBSTACLE)
        {
			isHandCollided = true;
			handGrenadeInput.Freeze();
			manager.PostCallbackWithFrameDelay(data.frameDelayReset, new Callback(HandleResetPosition));
        }
    }
	// Handlers end

	// Private begin
	private bool isGrenadeInputAvailable()
	{
		return inputCount < data.inputCountLob && !isHandCollided;
	}
    // Private end
}
