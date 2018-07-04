using UnityEngine;

public class LobMotor : MonoBehaviour, ILobInput {
    // Serialized fields
    public Transform root;

    public int speed = 400;
    public int speedFalloff = 50;
    public Vector3 direction = Vector3.one;
    public int forceFrameLength = 32;
    public int gravity = 100;
    private int forceFrameCount;

    private Vector3 velocity;

    private int inputDampening;

    private CallbackManager manager;

    private bool isInputAvailable;
    
    public void Start() {
        manager = new CallbackManager();
		isInputAvailable = true;
        
        inputDampening -= 20;
    }

    public void Update() {
        if (isInputAvailable && Input.GetKeyDown(KeyCode.D)) {
            Lob();
        }

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

        transform.Translate(velocity * FrameCounter.Instance.deltaTime, Space.Self);
		transform.position = CoreUtilities.NormalizePosition(transform.position);
    }

    public void Lob() {
        forceFrameCount = forceFrameLength;
        enabled = true;
        isInputAvailable = false;

        // TODO: fix magic numbers
		manager.PostCallbackWithFrameDelay(32, new Callback(HandleCallbackFired));
		manager.PostCallbackWithFrameDelay(256, new Callback(HandleResetPosition));
    }

    public void HandleCallbackFired() {
        isInputAvailable = true;
    }

    public void HandleResetPosition () {
		Freeze();
		transform.position = root.position;
    }

    public void Freeze() {
        enabled = false;
		velocity = Vector3.zero;
    }
}