using UnityEngine;

public class PlayerMotorData : ScriptableObject {
	public int velocityHorizontalGroundMax = 300;
	public int velocityHorizontalAirMax = 600;
	public int velocityVerticalJump = 1200;
	public int accelerationHorizontalAir = 50;
	public int gravity = 50;

	public void Awake () {
		// TODO: Assert that max values are evenly divisible by acceleration values.
	}
}
