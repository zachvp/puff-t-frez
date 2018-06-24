using UnityEngine;

// TODO: Clean up magic values
// TODO: Tie into replay system
public class PlayerHandMotor : MonoBehaviour, ITransform {
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
        // increase as the distance increases.
        var speed = snappiness * toTarget.magnitude;
        var velocity = toTarget.normalized * speed;

        newPos += velocity * FrameCounter.Instance.deltaTime;
        newPos.x = Mathf.RoundToInt(newPos.x);
        newPos.y = Mathf.RoundToInt(newPos.y);

        transform.position = newPos;
	}

    // ITransform
	public Vector3 GetPosition()
    {
		return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
		transform.position = position;
    }
}
