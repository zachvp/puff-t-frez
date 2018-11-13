using UnityEngine;

public class EnemyMotor : Motor<EnemyMotorData, PhysicsEntity>
{
    public EnemyMotor(PhysicsEntity e, Transform t)
        : base(e, t)
    {
        FrameCounter.Instance.OnFixedUpdate += HandleFixedUpdate;
    }

    public void HandleFixedUpdate(float deltaTime)
    {
        if (entity.collision.current.state.Below)
        {
            var v = new Vector2(-200, 1200);

            entity.SetVelocity(v);
        }

        //Debug.LogFormat("enemy collision: {0}", entity.collision.current.state);
    }
}
