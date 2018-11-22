using UnityEngine;

public class EnemyMotor : Motor<EnemyMotorData, PhysicsEntity>
{
	private CoreDirection targetDirection;

    public EnemyMotor(PhysicsEntity e, Transform t)
        : base(e, t)
    {
		targetDirection = new CoreDirection(Direction2D.RIGHT);

		FrameCounter.Instance.OnUpdate += HandleUpdate;
    }

	public void HandleUpdate(long count, float deltaTime)
    {
		var collision = entity.collision.current;

        if (collision.state.Below)
        {
			var v = data.baseVelocity;

			v.x *= targetDirection.Vector.x;

            entity.SetVelocity(v);
        }
		if (!collision.state.direction.IsEmptyHorizontal())
		{
			// Hit something on the left or right
			if (CoreDirection.IsSameHorizontal(targetDirection, collision.state.direction))
			{
				targetDirection.FlipHorizontal();
			}
		}

		ComputeDirection(entity.velocity);
    }
}
