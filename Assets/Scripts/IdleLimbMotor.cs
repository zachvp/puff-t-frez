using UnityEngine;

// TODO: Tie into replay system
public class IdleLimbMotor : Motor<IdleLimbMotorData, Entity>
{    
	public IdleLimbMotor(Entity e, Transform t)
		: base(e, t)
	{
		entity.SetPosition(root.position);

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public void HandleUpdate(long currentFrame, float deltaTime)
	{
		var toTarget = root.position - entity.Position;
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
}
