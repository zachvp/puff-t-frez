using UnityEngine;

public class PlayerGrenadeMotor : Motor<PlayerGrenadeMotorData, PhysicsEntity>
{
	public bool isGrabbable { get; private set; }

    public PlayerGrenadeMotor(PhysicsEntity e, Transform r)
        : base(e, r)
    {
		FrameCounter.Instance.OnUpdate += HandleUpdate;
    }

    // Public methods
	public void Launch(Vector2 additionalVelocity, CoreDirection d)
    {
		var v = d.Vector * data.baseVelocity;

		v += additionalVelocity * data.lobVelocityCoefficient;
		v.y = Mathf.Max(data.baseVelocity.y, v.y);

		entity.SetVelocity(v);        
    }

    // Handlers
	// todo: optimize so this disables when entity is disabled.
    public void HandleUpdate(long frame, float deltaTime)
    {
		if (entity.collision.current.IsColliding(Constants.Layers.OBSTACLE))
		{
			isGrabbable = true;
		}

        ComputeDirection(entity.velocity);
    }

    public void Grab()
	{
		isGrabbable = false;
	}
}
