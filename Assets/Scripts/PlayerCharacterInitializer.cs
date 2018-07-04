using UnityEngine;

public class PlayerCharacterInitializer : MonoBehaviour {
	public PlayerCharacterEngineEntity playerCharacterInstance;

	private PlayerKeyboardInputController inputController;
	private PlayerInputPlaybackController playback;
	private IdleLimbMotor handMotor;

    public void Awake() {
        var buffer = new InputBuffer();

		var instance = Instantiate(playerCharacterInstance, transform.position, Quaternion.identity);
		var collider = instance.GetComponent<BoxCollider2D>();
		var rigidBody = instance.GetComponent<Rigidbody2D>();
		var engine = new CharacterController2D(instance, collider, rigidBody);
		var motor = new PlayerMotor(engine);

		// TODO: This should look up an available input controller from the
		// connection manager/registry (yet to be created).
		playback = new PlayerInputPlaybackController(motor, instance, buffer);
		inputController = new PlayerKeyboardInputController(motor, buffer);

		// Spawn the limbs
		var hand = Instantiate(instance.hand, instance.handAnchor.position, Quaternion.identity);

		handMotor = new IdleLimbMotor(hand, instance.handAnchor);
	}
}
