using UnityEngine;

public class PlayerCharacterInitializer : MonoBehaviour
{
	public PlayerCharacterEntity bodyTemplate;
	public Entity handTemplate;
    public Entity footTemplate;
	public Entity handGrenadeTemplate;
    
    public void Awake()
	{
		// TODO: This should be a marionette input snapshot buffer
		var buffer = new InputBuffer<PlayerInputSnapshot>();

		var marionette = new PlayerMarionette();
        
		var bodyEntity = Instantiate(bodyTemplate, transform.position, Quaternion.identity);
		var bodyCollider = bodyEntity.GetComponent<BoxCollider2D>();
		var bodyRigidBody = bodyEntity.GetComponent<Rigidbody2D>();

		var bodyEngine = new CharacterController2D(bodyEntity, bodyCollider, bodyRigidBody);
		var bodyMotor = new PlayerMotor(bodyEntity, bodyEngine);

		// TODO: This should look up an available input controller from the
		// connection manager/registry (yet to be created).
		var inputController = new PlayerInputControllerKeyboard(marionette, buffer);

		// Spawn the limbs
		var handEntity = Instantiate(handTemplate, bodyEntity.handAnchor.position, Quaternion.identity);
		var handMotor = new IdleLimbMotor(handEntity, bodyEntity.handAnchor);

		var grenadeEntity = Instantiate(handGrenadeTemplate, handEntity.position, handEntity.rotation);
		var grenadeMotor = new LobMotor(grenadeEntity, handEntity.transform);
		var grenadeInput = new PlayerHandGrenadeInputControllerKeyboard(marionette);

        // Playback
		var playback = new InputPlaybackControllerPlayer(bodyMotor, bodyEntity, buffer);

        // Attach the limb input
		marionette.AttachBody(bodyMotor, bodyMotor, bodyEntity);
		marionette.AttachHand(handEntity);
		marionette.AttachHandGrenade(grenadeMotor, grenadeEntity);
	}
}
