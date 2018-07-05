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

		var bodyEngine = new CharacterController2D(bodyEntity, boxCollider, rigidBody);
		var bodyMotor = new PlayerMotor(bodyEngine);

		// TODO: This will likely need to be re-worked to account for all limbs
		var playback = new InputPlaybackControllerPlayer(bodyMotor, bodyEntity, buffer);

		// TODO: This should look up an available input controller from the
		// connection manager/registry (yet to be created).
		var inputController = new PlayerInputControllerKeyboard(marionette, buffer);

		// Spawn the limbs
		var handEntity = Instantiate(handTemplate, bodyEntity.handAnchor.position, Quaternion.identity);
		var handMotor = new IdleLimbMotor(handEntity, bodyEntity.handAnchor);

		var grenadeEntity = Instantiate(handGrenadeTemplate, handEntity.position, handEntity.rotation);
		var grenadeMotor = new LobMotor(grenadeEntity, handEntity.transform);
		var grenadeInput = new PlayerHandGrenadeInputControllerKeyboard(marionette);

        // Attach the limb input
		marionette.AttachBody(bodyMotor, bodyMotor);
		marionette.AttachHand(handEntity);
		marionette.AttachHandGrenade(grenadeMotor, grenadeEntity);
	}
}
