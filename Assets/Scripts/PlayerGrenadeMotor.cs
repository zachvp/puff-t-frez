using UnityEngine;
using System;

public class PlayerGrenadeMotor : LobMotor, IInputPlayerHandGrenade
{
	public EventHandler OnGrab;

	private CallbackManager manager;
	private PlayerGrenadeMotorData data;
	private MotorData baseData;

	public PlayerGrenadeMotor(Entity entityInstance, Transform rootInstance)
		: base(entityInstance, rootInstance)
	{
		manager = new CallbackManager();
		data = ScriptableObject.CreateInstance<PlayerGrenadeMotorData>();
	}

	public void SetBodyData(MotorData motorData)
	{
		baseData = motorData;
	}
    
	public void ApplyInput(InputSnapshot<HandGrenadeInput> input)
	{
		if (IsGrenadeInputAvailable() && input.pressed.launch)
        {
			var addVelocity = baseData.velocity * data.lobVelocityCoefficient;
			var flagDirection = input.held.direction;
            
            // Fall back to base input direction
			// TODO: Clean up these checks
			if (!FlagsHelper.IsSet(flagDirection, Direction2D.LEFT) &&
			    !FlagsHelper.IsSet(flagDirection, Direction2D.RIGHT))
			{
				flagDirection = baseData.direction;
			}
			if (FlagsHelper.IsSet(flagDirection, Direction2D.LEFT) &&
                FlagsHelper.IsSet(flagDirection, Direction2D.RIGHT))
            {
				Debug.AssertFormat(!(FlagsHelper.IsSet(flagDirection, Direction2D.LEFT) &&
                     FlagsHelper.IsSet(flagDirection, Direction2D.RIGHT)),
                   "Invalid direction given: {0}", flagDirection);

				Debug.LogFormat("both right and left set - fixing to be : {0}", baseData.direction);
                flagDirection = baseData.direction;
            }
            
			Lob(flagDirection, addVelocity);
        }
	}
    
	public void ApplyDeltaTime(float deltaTime)
	{
		
	}

    // Handlers
	public override void HandleTriggerStay(CollisionContext context)
	{
		base.HandleTriggerStay(context);

		if (state == State.FREEZE && context.IsColliding(Affinity.PLAYER))
        {
			Grab();
        }
	}

	public void Grab()
	{
		Events.Raise(OnGrab);
	}

	public override void Reset()
    {
		base.Reset();		
    }

    // Private
	private bool IsGrenadeInputAvailable()
    {
		return  state == State.NONE;
    }
}
