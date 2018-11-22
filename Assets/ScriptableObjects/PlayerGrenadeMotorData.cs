using UnityEngine;

public class PlayerGrenadeMotorData : ScriptableObject
{
	public Vector2 baseVelocity = new Vector2(1800, 1200);
	public int frameDelayReset = 64;
	public float lobVelocityCoefficient = 0.5f;
}
