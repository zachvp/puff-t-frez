using UnityEngine;

// TODO: Tie into replay system
public class IdleLimbMotor : Motor {
    private Transform root;
	private Entity entity;
	private IdleLimbMotorData data;
    
	public IdleLimbMotor(Entity entityInstance, Transform rootTransform) {
		data = ScriptableObject.CreateInstance<IdleLimbMotorData>();

		root = rootTransform;
		entity = entityInstance;

		entity.SetPosition(root.position);

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public void HandleUpdate(int currentFrame, float deltaTime) {
		var toTarget = root.position - entity.Position;
        var sqrDistance = toTarget.sqrMagnitude;
		var newPos = entity.Position;

        // Kind of a magic calculation. The idea is we want our speed to
        // increase as the distance increases. We then fudge that with a
        // user-defined param.
        var speed = data.snappiness * toTarget.magnitude;
        var velocity = toTarget.normalized * speed;

		newPos += velocity * deltaTime;
		newPos = CoreUtilities.NormalizePosition(newPos);

		entity.SetPosition(newPos);
	}
}
