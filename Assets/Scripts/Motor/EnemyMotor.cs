using UnityEngine;

public class EnemyMotor : Motor<EnemyMotorData, PhysicsEntity>
{
    public EnemyMotor(PhysicsEntity e, Transform t)
        : base(e, t)
    {
        
    }

    public override void HandleUpdate(long currentFrame, float deltaTime)
    {
        
    }
}
