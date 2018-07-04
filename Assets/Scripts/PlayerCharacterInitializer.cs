using UnityEngine;

public class PlayerCharacterInitializer : MonoBehaviour {
	public PlayerCharacterEntity playerTemplate;
    
    public void Awake() {
        var buffer = new InputBuffer();
        
		var bodyEntity = Instantiate(playerTemplate, transform.position, Quaternion.identity);
		var boxCollider = bodyEntity.GetComponent<BoxCollider2D>();
		var rigidBody = bodyEntity.GetComponent<Rigidbody2D>();
		var engine = new CharacterController2D(bodyEntity, boxCollider, rigidBody);
		var motor = new PlayerMotor(engine);
		var playback = new InputPlaybackControllerPlayer(motor, bodyEntity, buffer);

		// TODO: This should look up an available input controller from the
		// connection manager/registry (yet to be created).
		var inputController = new PlayerInputControllerKeyboard(motor, buffer);

		// Spawn the limbs
		var handEntity = Instantiate(bodyEntity.handTemplate, bodyEntity.handAnchor.position, Quaternion.identity);
		var handMotor = new IdleLimbMotor(handEntity, bodyEntity.handAnchor);
	}
}
