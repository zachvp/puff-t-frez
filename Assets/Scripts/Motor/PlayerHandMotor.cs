using UnityEngine;

public class PlayerHandMotor : Motor<PlayerHandMotorData, PhysicsEntity>
{
	public PlayerHandMotor(PhysicsEntity e, Transform t)
		: base(e, t)
	{
	}

	public void Punch(CoreDirection d)
	{
		Debug.Log("punch");
		entity.SetVelocity(d.Vector * data.punchSpeed);
	}
}
