using UnityEngine;

// TODO: Tie into replay system
public class IdleLimbMotor : Motor<IdleLimbMotorData, Entity>
{
	// Influenced means something else may be controlling this motor's position.
	protected enum State { IDLE, INFLUENCED }
	protected State state;

	public IdleLimbMotor(Entity e, Transform t)
		: base(e, t)
	{
		entity.SetPosition(root.position);

        FrameCounter.Instance.OnUpdate += HandleUpdate;
    }

    public void HandleUpdate(long currentFrame, float deltaTime)
	{
		if (state == State.IDLE)
		{
			HandleIdle(deltaTime);
		}
	}

	protected void HandleIdle(float deltaTime)
	{
		var toTarget = GetRootPosition() - entity.Position;
        var sqrDistance = toTarget.sqrMagnitude;
        var newPos = entity.Position;

		// Kind of a magic calculation. The idea is we want our speed to
        // increase as the distance increases. We then fudge that with a
        // user-defined param.
        var speed = data.snappiness * toTarget.magnitude;

        velocity = toTarget.normalized * speed;
        newPos += velocity * deltaTime;

        entity.SetPosition(newPos);
	}

	protected virtual Vector3 GetRootPosition()
	{
		return root.position;
	}
}
