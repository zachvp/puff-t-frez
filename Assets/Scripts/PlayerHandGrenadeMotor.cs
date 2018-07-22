using UnityEngine;

public class PlayerHandGrenadeMotor : LobMotor, IInputPlayerHandGrenade
{
	public EventHandler OnPickup;

	private CallbackManager manager;
    private int inputCount; // TODO: Should this be handled in hand grenade controller?
	private PlayerHandGrenadeMotorData data;
	private MotorData baseData;

    private bool isHandCollided;

	public PlayerHandGrenadeMotor(Entity entityInstance, Transform rootInstance)
		: base(entityInstance, rootInstance)
	{
		manager = new CallbackManager();
		data = ScriptableObject.CreateInstance<PlayerHandGrenadeMotorData>();

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
			//var addVelocity = baseData.velocity * data.lobVelocityCoefficient;
			var addVelocity = Vector3.zero;
			var flagDirection = Direction2D.NONE;

			if (input.pressed.launch)
            {
                Debug.LogFormat("launching!");
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
