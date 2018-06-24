using UnityEngine;

public class OffsetFromPlayerController : MonoBehaviour {
    public PlayerMotor motor;

    private Vector3 startPosition;
    private Vector3 otherPosition;

    void Awake() {
        startPosition = transform.localPosition;
        otherPosition = new Vector3(1, -2, 0);
    }

    void Update() {
        if (motor.GetDirection().x < 0) {
            transform.localPosition = otherPosition;
        } else if (motor.GetDirection().x > 0) {
            transform.localPosition = startPosition;
        }
    }
}
