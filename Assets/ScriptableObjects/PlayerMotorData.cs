using UnityEngine;

public class PlayerMotorData : ScriptableObject
{
	// TODO: Doc these values
	public int velocityHorizontalGroundMax = 1200;
	public int velocityHorizontalAirMax = 1600;
	public int accelerationHorizontalAir = 80;
	public int jumpCountMax = 1;
	public int velocityJumpImpulse = 3600;

	public int velocityJumpMax = 1400;
    public int velocityWallJumpHorizontal = 800;
    public int velocityWallJumpVertical = 3600;

	public float boundsMultiplierCrouchX = 1.5f;
	public float boundsMultiplierCrouchY = 0.5f;
	public float velocityThresholdMin = 0.01f;

	/// <summary>
	/// How many frames the additive jump will last.
	/// </summary>
	public int frameLimitJumpAdditive = 24;

    // How many frames the motor will be immune to gravity at the start of
    // the jump.
	public int frameLimitJumpGravityImmunity = 2;

	public CoreDirection initialDirection = new CoreDirection(new Vector2(1, -1));
}
