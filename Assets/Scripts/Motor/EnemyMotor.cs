using UnityEngine;

public class EnemyMotor : Motor<EnemyMotorData, PhysicsEntity>
{
    public EnemyMotor(PhysicsEntity e, Transform t)
        : base(e, t)
    {
        
    }

    public override void HandleFixedUpdate(float deltaTime)
    {
        if (entity.context.current.state.Below)
        {
            var v = new Vector2(-200, 1200);

            entity.SetVelocity(v);

        }

        Debug.LogFormat("enemy collision: {0}", entity.context.current.state);
    }
}
