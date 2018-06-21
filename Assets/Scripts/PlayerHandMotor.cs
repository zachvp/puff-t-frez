using UnityEngine;

// TODO: Clean up magic values
// TODO: Tie into replay system
public class PlayerHandMotor : MonoBehaviour, ITransform {
    public PlayerMotor root;

    public void Awake() {
        transform.position = root.transform.position;
    }

	public void Update() {
        var toTarget = root.transform.position - transform.position;
        var sqrDistance = toTarget.sqrMagnitude;
        var newPos = transform.position;
        var proportion = 0.9f;

        if (sqrDistance > Mathf.Pow(2, 15)) {
            proportion = 1;
        }
        
        var velocity = proportion * root.velocity.magnitude;

        if (velocity < 0.1) {
			velocity = 600;
        }

        newPos += toTarget.normalized * velocity * Time.deltaTime;
        newPos.x = Mathf.RoundToInt(newPos.x);
        newPos.y = Mathf.RoundToInt(newPos.y);

        transform.position = newPos;

        if (sqrDistance < 16) {
            transform.position = root.transform.position;
        }
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
