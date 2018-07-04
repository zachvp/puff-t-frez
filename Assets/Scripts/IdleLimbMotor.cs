using UnityEngine;

// TODO: Clean up magic values
// TODO: Tie into replay system
// TODO: Should use EngineEntity.
public class IdleLimbMotor {
    private Transform root;
	private EngineEntity entity;

	// TODO: Separate out into data class
    public float snappiness = 16;

	public IdleLimbMotor(EngineEntity engineEntity, Transform rootTransform) {
		root = rootTransform;
		entity = engineEntity;

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public void HandleUpdate(int currentFrame) {
		var toTarget = root.position - entity.position;
        var sqrDistance = toTarget.sqrMagnitude;
		var newPos = entity.position;

        // Kind of a magic calculation. The idea is we want our speed to
        // increase as the distance increases. We then fudge that with a
        // user-defined param.
        var speed = snappiness * toTarget.magnitude;
        var velocity = toTarget.normalized * speed;

        newPos += velocity * FrameCounter.Instance.deltaTime;
		newPos = CoreUtilities.NormalizePosition(newPos);

		entity.SetPosition(newPos);
	}
}
