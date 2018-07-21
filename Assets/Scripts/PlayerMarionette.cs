using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
using UnityEngine.UI;
public class PlayerMarionette : IPlayerMarionette
{
	private IInputPlayerBody bodyInput;
	private IMotor bodyMotor;
	private Entity bodyEntity;
        
	private IBehavior handBehavior;
	private Entity handEntity; 

	private IInputLob handGrenadeInput;
	private Entity handGrenadeEntity;
	private bool isHandCollided;

	private CallbackManager manager;
	private int inputCount; // TODO: Should this be handled in hand grenade controller?
	private PlayerMarionetteData data;
        
	public PlayerMarionette()
	{
		manager = new CallbackManager();
		data = ScriptableObject.CreateInstance<PlayerMarionetteData>();
	}

	public void AttachBody(IInputPlayerBody body, IMotor motor, Entity entity)
	{
		bodyInput = body;
		bodyMotor = motor;
		bodyEntity = entity;
	}

	public void AttachHand(IBehavior behavior, Entity entity)
	{
		handBehavior = behavior;
		handEntity = entity;
	}

	public void AttachHandGrenade(IInputLob lobInput, Entity entity)
	{
		handGrenadeInput = lobInput;
		handGrenadeEntity = entity;
		handGrenadeInput.Reset();

		handGrenadeEntity.OnTriggerEnter += HandleTriggerEnterHandGrenade;
	}

	// IPlayerMarionette begin
	public void ApplyPlayerInput(InputSnapshot<PlayerInput> input)
	{
		bodyInput.ApplyInput(input);
	}

	public void ApplyGrenadeInput(InputSnapshot<HandGrenadeInput> input)
	{
        if (IsGrenadeInputAvailable())
		{
			var addVelocity = bodyMotor.GetVelocity() * data.lobVelocityCoefficient;

            // TODO: Should be based on input direction not motor direction.
			if (input.released.launch)
            {
				Debug.LogFormat("launching!");
                var bodyDirection = CoreUtilities.ConvertFrom(bodyMotor.GetDirection());
                var direction = bodyDirection;

                if (FlagsHelper.IsSet(input.pressed.direction, Direction2D.RIGHT))
                {
                    FlagsHelper.Set(ref direction, Direction2D.RIGHT);
                }

				if (inputCount == 0)
				{
					isHandCollided = false;
					handGrenadeInput.Reset();
                    handBehavior.SetActive(false);
				}

                handGrenadeInput.Lob(direction, addVelocity);
				inputCount++;
            }
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
		handEntity.SetPosition(handGrenadeEntity.Position);
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
        }
		else if (layer == Constants.Layers.ENTITY && isHandCollided)
		{
			HandleResetPosition();
		}
    }
	// Handlers end

	// Private begin
	private bool IsGrenadeInputAvailable()
	{
		return inputCount < data.inputCountLob && !isHandCollided;
	}
    // Private end
}
