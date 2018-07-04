using UnityEngine;

// TODO: Clean up magic values
// TODO: Tie into replay system
// TODO: Should use EngineEntity.
public class IdleLimbMotor : MonoBehaviour, ITransform {
    public Transform root = null;

    public PlayerMotor motor = null;

    public float snappiness = 32;

    public void Awake() {
        transform.position = root.transform.position;
    }

	public void Update() {
        var toTarget = root.transform.position - transform.position;
        var sqrDistance = toTarget.sqrMagnitude;
        var newPos = transform.position;

        // Kind of a magic calculation. The idea is we want our speed to
        // increase as the distance increases. We then fudge that with a
        // user-defined param.
        var speed = snappiness * toTarget.magnitude;
        var velocity = toTarget.normalized * speed;

        newPos += velocity * FrameCounter.Instance.deltaTime;
		newPos = CoreUtilities.NormalizePosition(newPos);

        transform.position = newPos;
	}

    // ITransform
    public void SetPosition(Vector3 position)
    {
		transform.position = position;
    }

    public void SetLocalScale(Vector3 scale)
    {
        transform.localScale = scale;
    }
}
