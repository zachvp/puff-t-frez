using UnityEngine;

// TODO: Motor data scriptable object should be public serialized members too.
//       Can then pass data to motor classes.
using InControl;
public class PlayerCharacterInitializer : MonoBehaviour
{
	public PlayerCharacterEntity bodyTemplate;
	public Entity handTemplate;
    public Entity footTemplate;
	public Entity handGrenadeTemplate;

    public void Awake()
	{
		var buffer = new InputBuffer<InputSnapshot<PlayerInput>>();
		var grenadeBuffer = new InputBuffer<InputSnapshot<HandGrenadeInput>>();
		var handBuffer = new InputBuffer<InputSnapshot<HandInput>>();
        
		var bodyEntity = Instantiate(bodyTemplate, transform.position, Quaternion.identity);
		var bodyCollider = bodyEntity.GetComponent<BoxCollider2D>();
		var bodyRigidBody = bodyEntity.GetComponent<Rigidbody2D>();

		// TODO: Think only the body motor should care about the engine - try to init it there.
		var bodyEngine = new CharacterController2D(bodyEntity, bodyCollider, bodyRigidBody);
		var bodyMotor = new PlayerMotor(bodyEntity, transform, bodyEngine);

		// TODO: This should look up an available input controller from the
		// connection manager/registry (yet to be created).

		// Spawn the limbs
		var handEntity = Instantiate(handTemplate, bodyEntity.handAnchor.position, Quaternion.identity);
		var handMotor = new IdleLimbMotor(handEntity, bodyEntity.handAnchor);

		var grenadeEntity = Instantiate(handGrenadeTemplate, handEntity.Position, handEntity.Rotation);
		var grenadeMotor = new PlayerGrenadeMotor(grenadeEntity, handEntity.transform);

		var footEntity = Instantiate(footTemplate, bodyEntity.footAnchor.position, Quaternion.identity);
		var footMotor = new IdleLimbMotor(footEntity, bodyEntity.footAnchor);

        // Playback
		var playback = new InputPlaybackControllerPlayer(bodyMotor, bodyEntity, buffer);

        // Create the skeleton
		var skeleton = new PlayerSkeleton(bodyMotor, handMotor, footMotor, grenadeMotor);
		var marionette = new PlayerMarionette(skeleton);

		// TODO: this is a convenience hack for now
		if (InputManager.Devices.Count > 0)
		{
			var gamepadBody = new PlayerInputControllerGamepad(marionette, buffer);
			var gamepadGrenade = new PlayerGrenadeInputControllerGamepad(marionette, grenadeBuffer);
		}
		else
		{
			var keyboardBody = new PlayerInputControllerKeyboard(marionette, buffer);
			var keyboardGrenade = new PlayerGrenadeInputControllerKeyboard(marionette, grenadeBuffer);
		}
	}
}
