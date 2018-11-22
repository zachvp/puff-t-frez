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

		entity.tasks.Enqueue(new Callback(delegate {
			entity.SetVelocity(new Vector2(1800, 900));
		}));
        
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
