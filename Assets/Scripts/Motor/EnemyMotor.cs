using UnityEngine;

public class EnemyMotor : Motor<EnemyMotorData, Entity>
{
    public EnemyMotor(Entity e, Transform t)
        : base(e, t)
    {

    }

    public override void HandleUpdate(long currentFrame, float deltaTime)
    {
        base.HandleUpdate(currentFrame, deltaTime);

        velocity.y -= data.gravity;

        var newPosition = entity.Position + deltaTime * velocity;

        entity.SetPosition(newPosition);
    }
}
