using UnityEngine;

public class LobMotor : ILobInput {
	// TODO: This should live in scriptable object data class.
    public int speed = 400;
    public int speedFalloff = 50;
	public Vector3 direction = Vector3.right;
    public int forceFrameLength = 32;
    public int gravity = 100;
	public int inputDampening = -20;

	private Entity entity;
	private Transform root;
	private int forceFrameCount;
    private Vector3 velocity;
    private CallbackManager manager;
    private bool isInputAvailable;
    
	public LobMotor(Entity entityInstance, Transform rootInstance) {
		manager = new CallbackManager();

		entity = entityInstance;
		root = rootInstance;

		isInputAvailable = true;
	}

    public void HandleUpdate(int currentFrame, float deltaTime) {
        // Determine if force should be applied 
        if (forceFrameCount > 0) {
            var multiplier = (1 - (forceFrameCount / forceFrameLength));

            velocity = speed * direction * multiplier;
            --forceFrameCount;
        }

        // Apply the horizontal falloff
        if (Mathf.Abs(velocity.x) > 0) {
            velocity.x -= speedFalloff;
        }

        // Apply gravity
        velocity.y -= gravity;

        velocity += direction * inputDampening;

		var newPosition = velocity * deltaTime;
		newPosition = CoreUtilities.NormalizePosition(newPosition);

		entity.SetPosition(newPosition);
    }

    public void Lob() {
		// TODO: Input availability should be managed one level up - aka whoever calls Lob()
		if (isInputAvailable)
		{
			forceFrameCount = forceFrameLength;
			isInputAvailable = false;

			// TODO: This should be an interface method (IBehaviour?).
			entity.SetPosition(root.position);
			entity.enabled = true;


			// TODO: fix magic numbers
			// TODO: Same as isInputAvailable task above
			manager.PostCallbackWithFrameDelay(32, new Callback(HandleCallbackFired));
			manager.PostCallbackWithFrameDelay(256, new Callback(HandleResetPosition));
		}
    }

    public void HandleCallbackFired() {
        isInputAvailable = true;
    }

    public void HandleResetPosition () {
		Freeze();
		entity.SetPosition(root.position);
    }

    public void Freeze() {
        entity.enabled = false;
		velocity = Vector3.zero;
    }
}