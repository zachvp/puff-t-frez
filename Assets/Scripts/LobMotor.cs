using UnityEngine;

public class LobMotor : Motor, ILobInput
{
	private Entity entity;
	private Transform root;
	private int forceFrameCount;
    private Vector3 velocity;
	private Vector3 direction;
	private LobMotorData data;
    
	private enum State { FOLLOW, LOB }
	private State state;
    
	public LobMotor(Entity entityInstance, Transform rootInstance)
	{
		data = ScriptableObject.CreateInstance<LobMotorData>();

		state = State.FOLLOW;
		entity = entityInstance;
		root = rootInstance;

		HandleFrameUpdate(HandleUpdate);
	}

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

				velocity = data.speed * direction.normalized * multiplier;
                --forceFrameCount;
            }

            // Apply gravity
			velocity.y -= data.gravity;

            var newPosition = entity.position + deltaTime * velocity;
            newPosition = CoreUtilities.NormalizePosition(newPosition);

            entity.SetPosition(newPosition);
		}
    }

    // ILobmotor begin
	public void Lob(Direction2D lobDirection)
	{
		Debug.AssertFormat(lobDirection == Direction2D.LEFT || lobDirection == Direction2D.RIGHT, "Invalid direction given: {0}", direction);

		if (lobDirection == Direction2D.RIGHT)
		{
			direction = new Vector3(1, 1, 0);
		}
		else
		{
			direction = new Vector3(-1, 1, 0);
		}

		forceFrameCount = data.forceFrameLength;
		state = State.LOB;
    }
        
    public void Freeze()
	{
		ClearFrameUpdate(HandleUpdate);
    }

	public void Reset()
	{
		state = State.FOLLOW;
		HandleFrameUpdate(HandleUpdate);
	}
    // ILobMotor end
}