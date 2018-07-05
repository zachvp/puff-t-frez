using UnityEngine;

// TODO: Clean up magic values
// TODO: Tie into replay system
public class IdleLimbMotor : Motor {
    private Transform root;
	private Entity entity;

	// TODO: Separate out into data class
    public float snappiness = 16;

	public IdleLimbMotor(Entity engineEntity, Transform rootTransform) {
		root = rootTransform;
		entity = engineEntity;

		entity.SetPosition(root.position);

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public void HandleUpdate(int currentFrame, float deltaTime) {
		var toTarget = root.position - entity.position;
        var sqrDistance = toTarget.sqrMagnitude;
		var newPos = entity.position;

        // Kind of a magic calculation. The idea is we want our speed to
        // increase as the distance increases. We then fudge that with a
        // user-defined param.
        var speed = snappiness * toTarget.magnitude;
        var velocity = toTarget.normalized * speed;

		newPos += velocity * deltaTime;
		newPos = CoreUtilities.NormalizePosition(newPos);

		entity.SetPosition(newPos);
	}
}
