using UnityEngine;

public class LobMotor : Motor, IInputLob
{
	protected Transform root;
	protected Vector3 direction;

	private int forceFrameCount;
	private int additiveSpeed; 

	private LobMotorData data;
    
	protected enum State { NONE, LAUNCHED, FREEZE }
	protected State state;
    
	public LobMotor(Entity entityInstance, Transform rootInstance)
	{
		data = ScriptableObject.CreateInstance<LobMotorData>();
		additiveSpeed = 1;

		entity = entityInstance;
		root = rootInstance;

		entity.OnTriggerEnter += HandleTriggerEnter;
		entity.OnTriggerStay += HandleTriggerStay;
	}

    // Handlers begin
	public virtual void HandleUpdate(long currentFrame, float deltaTime)
	{
		if (state == State.NONE)
		{
			entity.SetPosition(root.position);
			Debug.LogFormat("FOLLOW");
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
				velocity.x *= direction.x;
                
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
	public void Lob(Direction2D lobDirection, Vector3 baseVelocity)
	{
		forceFrameCount = data.forceFrameLength;
		state = State.LAUNCHED;

		entity.SetActive(true);
		HandleFrameUpdate(HandleUpdate);

		Debug.AssertFormat(FlagsHelper.IsSet(lobDirection, Direction2D.LEFT) ||
		                   FlagsHelper.IsSet(lobDirection, Direction2D.RIGHT),
		                   "Invalid direction given: {0}", lobDirection);

		direction = CoreUtilities.Convert(lobDirection);

        // To handle cases when the motor is lobbed from an object in motion,
        // we add the given velocity to our force frames.
		additiveSpeed = Mathf.RoundToInt(Mathf.Abs(baseVelocity.x));
    }

	public void Lob(Vector3 direction, Vector3 baseVelocity)
	{
		var converted = CoreUtilities.Convert(direction);

		Lob(converted, baseVelocity);
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