using UnityEngine;

public class PlayerMotorData : ScriptableObject {
	// TODO: Doc these values
	public int velocityHorizontalGroundMax = 1600;
	public int velocityHorizontalAirMax = 2000;
	public int accelerationHorizontalAir = 200;
	public int jumpCountMax = 1;
	public int velocityJumpImpulse = 200;

	public int velocityJumpMax = 1400;
    public int velocityWallJumpHorizontal = 400;
    public int velocityWallJumpVertical = 3600;
	public int gravity = 200;

	/// <summary>
	/// How many frames the additive jump will last.
	/// </summary>
	public int frameLimitJumpAdditive = 24;
	public int frameLimitJumpGravityImmunity = 2;
}
