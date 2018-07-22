using UnityEngine;
using System;

public class PlayerGrenadeMotor : LobMotor
{
	public EventHandler OnGrab;

	private CallbackManager manager;
	private PlayerGrenadeMotorData data;
	private int playerHitCount;

	public PlayerGrenadeMotor(Entity entityInstance, Transform rootInstance)
		: base(entityInstance, rootInstance)
	{
		manager = new CallbackManager();
		data = ScriptableObject.CreateInstance<PlayerGrenadeMotorData>();
	}
    
	public void ApplyInput(InputSnapshot<HandGrenadeInput> input)
	{
		if (IsGrenadeInputAvailable() && input.pressed.launch)
        {
			var addVelocity = input.held.data.velocity * data.lobVelocityCoefficient;
			var flagDirection = input.held.data.direction;
            
            // Fall back to base input direction
			// TODO: Clean up these checks
			if (!FlagsHelper.IsSet(flagDirection, Direction2D.LEFT) &&
			    !FlagsHelper.IsSet(flagDirection, Direction2D.RIGHT))
			{
				flagDirection = input.held.data.direction;
			}
			if (FlagsHelper.IsSet(flagDirection, Direction2D.LEFT) &&
                FlagsHelper.IsSet(flagDirection, Direction2D.RIGHT))
            {
				Debug.AssertFormat(!(FlagsHelper.IsSet(flagDirection, Direction2D.LEFT) &&
                     FlagsHelper.IsSet(flagDirection, Direction2D.RIGHT)),
                   "Invalid direction given: {0}", flagDirection);

				flagDirection = input.held.data.direction;
            }


			Lob(flagDirection, addVelocity);
        }
	}
    
	public void ApplyDeltaTime(float deltaTime)
	{
		
	}

    // Handlers
	public override void HandleTriggerEnter(CollisionContext context)
	{
		base.HandleTriggerEnter(context);

		if (context.IsColliding(Affinity.PLAYER))
        {
			playerHitCount++;

			if (playerHitCount > 1)
			{
				manager.PostIdempotentCallback(2, new Callback(Grab));
			}
        }
	}

	public override void HandleTriggerStay(CollisionContext context)
	{
		base.HandleTriggerStay(context);

		if (context.IsColliding(Affinity.PLAYER))
        {
            if (state == State.FREEZE)
            {
                manager.PostIdempotentCallback(2, new Callback(Grab));
            }
        }
	}

	public void Grab()
	{
		playerHitCount = 0;
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
