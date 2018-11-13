using UnityEngine;

public class PlayerGrenadeMotor : LobMotor<PlayerGrenadeMotorData>
{
	public EventHandler<CollisionContext> OnGrab;

	private CallbackManager manager;

    // Tracks the number of times the motor has touched the player.
	private int playerTouchCount;
    
	public PlayerGrenadeMotor(PhysicsEntity entityInstance, Transform rootInstance)
		: base(entityInstance, rootInstance)
	{
		manager = new CallbackManager();
	}

	public override void HandleUpdate(long currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

		if (entity.collision.current.IsColliding(Constants.Layers.OBSTACLE) &&
            entity.collision.current.IsColliding(Affinity.PLAYER))
        {
            if (state == State.FREEZE)
            {
                EventHandler c = delegate
                {
					Grab(entity.collision.current);
                };

                manager.PostIdempotentCallback(2, new Callback(c));
            }
        }

		if (state == State.LAUNCHED &&
		    !entity.collision.previous.IsColliding(Affinity.PLAYER) &&
		    entity.collision.current.IsColliding(Affinity.PLAYER))
        {
			playerTouchCount++;

			if (playerTouchCount > 1)
			{
				Grab(entity.collision.current);
			}
        }
	}

	public void ApplyInput(InputSnapshot<HandGrenadeInput> input)
	{
		// TODO: Move this check to Update() and simply set input member variable
		if (IsGrenadeInputAvailable() && input.pressed.launch)
        {
			var addVelocity = input.held.data.velocity * data.lobVelocityCoefficient;

            // Fall back to base input direction if there's no input direction.
			if (input.held.direction.IsEmpty())
			{
				input.held.direction.Update(input.held.data.direction);
			}

			Debug.AssertFormat(FlagsHelper.IsSet(input.held.direction.Flags,
                                     Direction2D.HORIZONTAL,
                                     Logical.OR),
			                   "Invalid direction given: {0}", input.held.direction.Flags);

			Lob(input.held.direction, addVelocity);
        }
	}

	// Private
	private void Grab(CollisionContext context)
	{
		playerTouchCount = 0;
		Events.Raise(OnGrab, context);
	}

	private bool IsGrenadeInputAvailable()
    {
		return state == State.NONE && playerTouchCount == 0;
    }
}
