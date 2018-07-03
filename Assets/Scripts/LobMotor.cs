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

    // TODO: Nerf reset

    public void Start() {
        manager = new CallbackManager();
        
        inputDampening -= 20;

        //Lob();
    }

    public void Update() {
        if (/*isInputAvailable && */Input.GetKeyDown(KeyCode.D)) {
            Lob();
			manager.PostCallbackWithFrameDelay(32, new Callback(HandleCallbackFired));
			manager.PostCallbackWithFrameDelay(32, new Callback(HandleCallbackFired));
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

        // TODO: Round positions
    }

    public void Lob() {
        forceFrameCount = forceFrameLength;
        enabled = true;
        isInputAvailable = false;

        // TODO: fix magic numbers
        //manager.PostCallbackWithFrameDelay(32, HandleCallbackFired);
        //manager.PostCallbackWithFrameDelay(256, HandleResetPosition);
    }

    public void HandleCallbackFired() {
        isInputAvailable = true;
    }

    public void HandleResetPosition () {
        transform.position = root.position;
		Freeze();
    }

    public void Freeze() {
        enabled = false;
    }
}