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
	public void Launch(Vector2 baseVelocity, CoreDirection d)
    {
		var v = d.Vector * data.speed;

		v.y = 1200;
		v += baseVelocity * 0.5f;

		v.y = Mathf.Max(1200, v.y);

		entity.SetVelocity(v);
        
        Debug.LogFormat("pressed launch; vel: {0}", v);
    }

    // Handlers
	// todo: optimize so this disables when entity is disabled.
    public void HandleUpdate(long frame, float deltaTime)
    {
		if (entity.collision.current.IsColliding(Constants.Layers.OBSTACLE))
		{
			Debug.Log("collide with obstacle");
			isGrabbable = true;
		}

        ComputeDirection(entity.velocity);
    }

    public void Grab()
	{
		isGrabbable = false;
	}
}
