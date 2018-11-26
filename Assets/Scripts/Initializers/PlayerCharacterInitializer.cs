using UnityEngine;
using InControl;

// TODO: Motor data scriptable object should be public serialized members too.
//       Can then pass data to motor classes.
public class PlayerCharacterInitializer : MonoBehaviour
{
	public PlayerCharacterEntity bodyTemplate;
	public Entity handTemplate;
    public Entity footTemplate;
	public PhysicsEntity handGrenadeTemplate;

    public void Awake()
	{
		var buffer = new InputBuffer<InputSnapshot<PlayerInput>>();
		var grenadeBuffer = new InputBuffer<InputSnapshot<HandGrenadeInput>>();
		var combatHandBuffer = new InputBuffer<InputSnapshot<CombatHandInput>>();
        
		var bodyEntity = Instantiate(bodyTemplate, transform.position, Quaternion.identity);
		var bodyCollider = bodyEntity.GetComponent<BoxCollider2D>();
		var bodyRigidBody = bodyEntity.GetComponent<Rigidbody2D>();

		var bodyMotor = new PlayerMotor(bodyEntity, transform);

		// Spawn the limbs
		// todo: rename to 'idleHandEntity'
		var handEntity = Instantiate(handTemplate, bodyEntity.handAnchorRight.position, Quaternion.identity);
		var idleHandMotor = new IdleLimbMotor(handEntity, bodyEntity.handAnchorRight);

		var grenadeEntity = Instantiate(handGrenadeTemplate, handEntity.Position, handEntity.Rotation);
		var grenadeMotor = new PlayerGrenadeMotor(grenadeEntity, handEntity.transform);

		var handCombatMotor = new PlayerHandMotor(grenadeEntity, handEntity.transform);

		var footEntity = Instantiate(footTemplate, bodyEntity.footAnchorRight.position, Quaternion.identity);
		var footMotor = new IdleLimbMotor(footEntity, bodyEntity.footAnchorRight);

        // Playback
		var playback = new InputPlaybackControllerPlayer(bodyMotor, bodyEntity, buffer);

        // Create the skeleton
		var skeleton = new PlayerSkeleton();
		skeleton.AttachBody(bodyMotor)
                .AttachHand(idleHandMotor)
                .AttachFoot(footMotor)
                .AttachGrenade(grenadeMotor)
		        .AttachCombatHand(handCombatMotor);

		skeleton.SetActive(Limb.COMBAT_HAND, false);

		var marionette = new PlayerMarionette(skeleton);

        // Set affinity for all limbs
		bodyEntity.SetAffinity(Affinity.PLAYER);
		handEntity.SetAffinity(Affinity.PLAYER);
		grenadeEntity.SetAffinity(Affinity.PLAYER);
		footEntity.SetAffinity(Affinity.PLAYER);
        
        // TODO: This should look up an available input controller from the
        // connection manager/registry (yet to be created).
        // This is a convenience hack for now
        if (InputManager.Devices.Count > 0)
		{
			var gamepadBody = new PlayerInputControllerGamepad(marionette, buffer);
			var gamepadGrenade = new PlayerGrenadeInputControllerGamepad(marionette, grenadeBuffer);
		}
		else
		{
			var keyboardBody = new PlayerInputControllerKeyboard(marionette, buffer);
			var keyboardGrenade = new PlayerGrenadeInputControllerKeyboard(marionette, grenadeBuffer);
			var keyboardCombatHand = new PlayerInputControllerCombatHand(marionette, combatHandBuffer);
		}
	}
}
