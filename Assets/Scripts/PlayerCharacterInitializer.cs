using UnityEngine;

public class PlayerCharacterInitializer : MonoBehaviour {
	public GameObject playerBodyInstance;

	private PlayerKeyboardInputController inputController;

    public void Awake() {
        var buffer = new InputBuffer();

		var instance = Instantiate(playerBodyInstance, transform.position, Quaternion.identity);
		var collider = instance.GetComponent<BoxCollider2D>();
		var rigidBody = instance.GetComponent<Rigidbody2D>();
		var engine = new CharacterController2D(instance, collider, rigidBody);
		var motor = new PlayerMotor(instance, engine);

		inputController = new PlayerKeyboardInputController(motor, buffer);
	}

    // Create the input controller and player motor dynamically
    // Need
}
