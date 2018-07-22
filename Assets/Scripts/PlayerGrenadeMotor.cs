using UnityEngine;

public class PlayerGrenadeMotor : LobMotor, IInputPlayerHandGrenade
{
	public EventHandler OnPickup;

	private CallbackManager manager;
    private int inputCount;
	private PlayerGrenadeMotorData data;
	private MotorData baseData;

    private bool isHandCollided;

	public PlayerGrenadeMotor(Entity entityInstance, Transform rootInstance)
		: base(entityInstance, rootInstance)
	{
		manager = new CallbackManager();
		data = ScriptableObject.CreateInstance<PlayerGrenadeMotorData>();

		entity.OnTriggerEnter += HandleTriggerEnterHandGrenade;
	}

	public void SetBodyData(MotorData motorData)
	{
		baseData = motorData;
	}
    
	public void ApplyInput(InputSnapshot<HandGrenadeInput> input)
	{
		if (IsGrenadeInputAvailable())
        {
			var addVelocity = baseData.velocity * data.lobVelocityCoefficient;
			var flagDirection = Direction2D.NONE;

			if (input.pressed.launch)
            {
                if (FlagsHelper.IsSet(input.pressed.direction, Direction2D.RIGHT))
                {
					FlagsHelper.Set(ref flagDirection, Direction2D.RIGHT);
					FlagsHelper.Unset(ref flagDirection, Direction2D.LEFT);
                }
				if (FlagsHelper.IsSet(input.pressed.direction, Direction2D.LEFT))
				{
					FlagsHelper.Set(ref flagDirection, Direction2D.LEFT);
					FlagsHelper.Unset(ref flagDirection, Direction2D.RIGHT);
				}
                
                // Fall back to base input direction
				if (!FlagsHelper.IsSet(flagDirection, Direction2D.LEFT) &&
				    !FlagsHelper.IsSet(flagDirection, Direction2D.RIGHT))
				{
					flagDirection = baseData.direction;
					FlagsHelper.Unset(ref flagDirection, Direction2D.UP);
					FlagsHelper.Unset(ref flagDirection, Direction2D.DOWN);
				}

				Lob(flagDirection, addVelocity);
                inputCount++;
            }
        }
	}
    
	public void ApplyDeltaTime(float deltaTime)
	{
		
	}

    // Handlers
	public void HandleTriggerEnterHandGrenade(Collider2D collider)
    {
        var layer = collider.gameObject.layer;

        if (layer == Constants.Layers.OBSTACLE)
        {
            isHandCollided = true;
            Freeze();
        }
        else if (layer == Constants.Layers.ENTITY && isHandCollided)
        {
			Events.Raise(OnPickup);
        }
    }

	public override void Reset()
    {
		base.Reset();
		
        inputCount = 0;
        isHandCollided = false;
    }

    // Private
	private bool IsGrenadeInputAvailable()
    {
        return inputCount < data.inputCountLob && !isHandCollided;
    }
}
