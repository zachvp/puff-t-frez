using UnityEngine;
using System;

public class PlayerGrenadeMotor : LobMotor, IInputPlayerHandGrenade
{
	public EventHandler OnGrab;

	private CallbackManager manager;
	private int inputCount;// TODO: Remove
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
		if (IsGrenadeInputAvailable())
        {
			var addVelocity = baseData.velocity * data.lobVelocityCoefficient;
			var flagDirection = Direction2D.NONE;

			if (input.pressed.launch)
            {
				Debug.LogFormat("grenade input fired");

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
		Debug.LogFormat("resetting");
		base.Reset();
		
        inputCount = 0;
    }

    // Private
	private bool IsGrenadeInputAvailable()
    {
		return inputCount < data.inputCountLob &&
               state == State.NONE;
    }
}
