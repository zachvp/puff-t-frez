using UnityEngine;

public class PlayerGrenadeMotor : Motor<PlayerGrenadeMotorData, PhysicsEntity>
{
    public PlayerGrenadeMotor(PhysicsEntity e, Transform r)
        : base(e, r)
    {

    }

    public void Launch()
    {

    }

    public void ApplyInput(InputSnapshot<HandGrenadeInput> input)
    {
        if (input.pressed.launch)
        {
            Debug.Log("pressed launch");
        }
    }
}
