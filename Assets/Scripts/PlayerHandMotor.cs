using UnityEngine;
using System.Collections;

public class PlayerHandMotor : MonoBehaviour {
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
            velocity = 800;
        }

        newPos += toTarget.normalized * velocity * Time.deltaTime;
        newPos.x = Mathf.RoundToInt(newPos.x);
        newPos.y = Mathf.RoundToInt(newPos.y);

        transform.position = newPos;

        if (sqrDistance < 24) {
            transform.position = root.transform.position;
        }
	}
}
