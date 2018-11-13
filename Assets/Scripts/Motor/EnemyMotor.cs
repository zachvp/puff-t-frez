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
            var d = Vector2.up;
            var v = d * 6400;

            entity.SetVelocity(v);
        }

        Debug.LogFormat("enemy collision: {0}", entity.context.current.state);
    }
}
