using UnityEngine;

public class LobMotor : Motor, IInputLob
{
	private Entity entity;
	private Transform root;
	private int forceFrameCount;
    private Vector3 velocity;
	private Vector3 direction;
	private int additiveSpeed; 
	private LobMotorData data;
    private LobMotorData backupData;
    
	private enum State { FOLLOW, LOB }
	private State state;
    
	public LobMotor(Entity entityInstance, Transform rootInstance)
	{
		data = ScriptableObject.CreateInstance<LobMotorData>();
		backupData = ScriptableObject.CreateInstance<LobMotorData>();
		additiveSpeed = 1;

		state = State.FOLLOW;
		entity = entityInstance;
		root = rootInstance;
	}

    // Handlers begin
    public void HandleUpdate(int currentFrame, float deltaTime)
	{
		if (state == State.FOLLOW)
		{
			entity.SetPosition(root.position);
		}
		else
		{
			// Determine if force should be applied 
            if (forceFrameCount > 0)
            {
                var multiplier = (1 - (forceFrameCount / data.forceFrameLength));

				velocity = (data.speed + additiveSpeed) * direction * multiplier;

                --forceFrameCount;
			} else {
				additiveSpeed = 1;
			}

            // Apply gravity
			velocity.y -= data.gravity;

            var newPosition = entity.position + deltaTime * velocity;
            newPosition = CoreUtilities.NormalizePosition(newPosition);

            entity.SetPosition(newPosition);
		}
    }
    // Handlers end

    // ILobmotor begin
	public void Lob(Direction2D lobDirection, Vector3 baseVelocity)
	{
		forceFrameCount = data.forceFrameLength;
        state = State.LOB;

		entity.SetActive(true);
		HandleFrameUpdate(HandleUpdate);

		Debug.AssertFormat(lobDirection == Direction2D.LEFT ||
		                   lobDirection == Direction2D.RIGHT,
		                   "Invalid direction given: {0}", direction);

		if (lobDirection == Direction2D.RIGHT)
		{
			direction = data.directionRight;
		}
		else
		{
			direction = data.directionLeft;
		}

        // To handle cases when the motor is lobbed from an object in motion,
        // we add the given velocity to our force frames.
		additiveSpeed = Mathf.RoundToInt(Mathf.Abs(baseVelocity.x));
    }
        
    public void Freeze()
	{
		velocity = Vector3.zero;
		ClearFrameUpdate(HandleUpdate);
    }

	public void Reset()
	{
		data.speed = backupData.speed;

		state = State.FOLLOW;
		ClearFrameUpdate(HandleUpdate);
		entity.SetActive(false);
		entity.SetPosition(root.position);
	}
    // ILobMotor end
}