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
	public void ApplyPlayerInput(InputSnapshot<PlayerInput> snapshot)
	{
		bodyInput.ApplyInput(snapshot);
	}

	public void ApplyGrenadeInput(InputSnapshot<HandGrenadeInput> snapshot)
	{
		var addVelocity = bodyMotor.GetVelocity() * data.lobVelocityCoefficient;

        // TODO: Should be based on input direction not motor direction.
        if (bodyMotor.GetDirection().x < 0)
        {
            handGrenadeInput.Lob(Direction2D.LEFT, addVelocity);
        }
        else
        {
            handGrenadeInput.Lob(Direction2D.RIGHT, addVelocity);
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
	private bool isGrenadeInputAvailable()
	{
		return inputCount < data.inputCountLob && !isHandCollided;
	}
    // Private end
}
