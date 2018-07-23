using UnityEngine;

public class LobMotor<T> : Motor<T, Entity>, IInputLob where T : LobMotorData
{
	private int forceFrameCount;
	private int additiveSpeed; 

	protected enum State { NONE, LAUNCHED, FREEZE }
	protected State state;
    
	public LobMotor(Entity e, Transform t)
		: base(e, t)
	{
		additiveSpeed = 1;
        
		entity.OnTriggerEnter += HandleTriggerEnter;
		entity.OnTriggerStay += HandleTriggerStay;
	}

    // Handlers begin
	public virtual void HandleUpdate(long currentFrame, float deltaTime)
	{
		if (state == State.NONE)
		{
			entity.SetPosition(root.position);
		}
		else if (state == State.LAUNCHED)
		{
			// Determine if force should be applied 
            if (forceFrameCount > 0)
            {
                var multiplier = 1 - (forceFrameCount / data.forceFrameLength);
				var speed = data.speed + additiveSpeed;

				velocity = speed * data.multiplier * multiplier;
                
                // Set the velocity direction based on the input direction.
				velocity.x *= direction.vector.x;
				Debug.LogFormat("direction.vector: {0}", direction.vector);
                
                --forceFrameCount;
			}

            // Apply gravity
			velocity.y -= data.gravity;

            var newPosition = entity.Position + deltaTime * velocity;

            entity.SetPosition(newPosition);
		}
    }

	public virtual void HandleTriggerEnter(CollisionContext context)
    {
		if (context.IsColliding(Constants.Layers.OBSTACLE))
        {
            if (state != State.FREEZE)
            {
                Freeze();
            }
        }
    }

	public virtual void HandleTriggerStay(CollisionContext context)
	{
		
	}
    
    // Handlers end

    // ILobmotor begin
	public void Lob(CoreDirection lobDirection, Vector3 baseVelocity)
	{
		Debug.AssertFormat(lobDirection.flags != Direction2D.NONE, "illegal direction passed");

		forceFrameCount = data.forceFrameLength;
		state = State.LAUNCHED;

		entity.SetActive(true);
		direction.Update(lobDirection);
		HandleFrameUpdate(HandleUpdate);
        
        // To handle cases when the motor is lobbed from an object in motion,
        // we add the given velocity to our force frames.
		additiveSpeed = Mathf.RoundToInt(Mathf.Abs(baseVelocity.x));
    }
        
	public virtual void Freeze()
	{
		velocity = Vector3.zero;
		state = State.FREEZE;
    }

	public virtual void Reset()
	{
		forceFrameCount = data.forceFrameLength;

		state = State.NONE;
		ClearFrameUpdate(HandleUpdate);
		entity.SetActive(false);
		entity.SetPosition(root.position);
	}
    // ILobMotor end
}