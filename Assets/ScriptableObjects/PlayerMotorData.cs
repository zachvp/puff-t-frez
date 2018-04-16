using UnityEngine;

public class PlayerMotorData : ScriptableObject {
	// TODO: Doc these values
	public int velocityHorizontalGroundMax = 400;
	public int velocityHorizontalAirMax = 600;
	public int accelerationHorizontalAir = 50;
	public int jumpCountMax = 1;
	public int velocityJumpImpulse = 200;
	public int velocityJumpAdditive = 100;
	public int velocityJumpMax = 1200;
	public int gravity = 50;

	/// <summary>
	/// How many frames the additive jump will last.
	/// </summary>
	public int frameLimitJumpAdditive = 10;
	public int frameLimitJumpGravityImmunity = 2;

	public void Awake () {
		// TODO: Assert that max values are evenly divisible by acceleration values.
	}
}
