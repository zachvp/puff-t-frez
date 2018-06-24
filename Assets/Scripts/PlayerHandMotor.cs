using UnityEngine;

// TODO: Clean up magic values
// TODO: Tie into replay system
public class PlayerHandMotor : MonoBehaviour, ITransform {
    public Transform root;

    public PlayerMotor motor;

    public void Awake() {
        transform.position = root.transform.position;
    }

	public void Update() {
        var toTarget = root.transform.position - transform.position;
        var sqrDistance = toTarget.sqrMagnitude;
        var newPos = transform.position;

        // Kind of a magic calculation. The idea is we want our speed to
        // increase as the distance increases.
        var proportion = 3 * (sqrDistance / Mathf.Pow(2, 16));
        
        // Computed speed is a mix of distance to target and velocity of
        // target.
        var speed = proportion * motor.velocity.magnitude;

        // Still want to be moving towards target even when target is
        // stopped or at low velocity
        if (speed < 0.1) {
            speed = 400;
        }

        newPos += toTarget.normalized * speed * FrameCounter.Instance.deltaTime;
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
