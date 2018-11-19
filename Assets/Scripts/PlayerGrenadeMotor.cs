using UnityEngine;

public class PlayerGrenadeMotor : Motor<PlayerGrenadeMotorData, PhysicsEntity>
{
    public PlayerGrenadeMotor(PhysicsEntity e, Transform r)
        : base(e, r)
    {

    }

    // Public methods
    public void Launch()
    {

    }

    public void ApplyInput(InputSnapshot<HandGrenadeInput> input)
    {
        if (input.pressed.launch)
        {
            Debug.Log("pressed launch");
            var d = new Vector2(1, 0.8f);
            var v = d * data.speed;

            entity.SetVelocity(v);
        }
    }

    // Handlers
    public void HandleUpdate(long frame, float deltaTime)
    {
        ComputeDirection(entity.velocity);
    }

}
