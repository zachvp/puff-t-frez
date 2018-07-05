using UnityEngine;

public class PlayerCharacterInitializer : MonoBehaviour {
	public PlayerCharacterEntity playerTemplate;
	public Entity handTemplate;
    public Entity footTemplate;
	public Entity handGrenadeTemplate;
    
    public void Awake() {
        var buffer = new InputBuffer();
		var marionette = new PlayerMarionette();
        
		var bodyEntity = Instantiate(playerTemplate, transform.position, Quaternion.identity);
		var boxCollider = bodyEntity.GetComponent<BoxCollider2D>();
		var rigidBody = bodyEntity.GetComponent<Rigidbody2D>();

		var engine = new CharacterController2D(bodyEntity, boxCollider, rigidBody);
		var motor = new PlayerMotor(engine);
		var playback = new InputPlaybackControllerPlayer(motor, bodyEntity, buffer);

		// TODO: This should look up an available input controller from the
		// connection manager/registry (yet to be created).
		var inputController = new PlayerInputControllerKeyboard(marionette, buffer);

		// Spawn the limbs
		var handEntity = Instantiate(handTemplate, bodyEntity.handAnchor.position, Quaternion.identity);
		var handMotor = new IdleLimbMotor(handEntity, bodyEntity.handAnchor);

		var handGrenade = Instantiate(handGrenadeTemplate, handEntity.position, handEntity.rotation);
		var grenadeMotor = new LobMotor(handGrenade, handEntity.transform);
		var grenadeInput = new PlayerHandGrenadeInputControllerKeyboard(marionette);

        // Attach the limb input
		marionette.AttachBody(motor, motor);
		marionette.AttachHandGrenade(grenadeMotor);
	}
}
