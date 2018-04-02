using UnityEngine;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMotor : MonoBehaviour, IPlayerInput
{
	// Reference to the character controller engine.
	private CharacterController2D engine;

	// The motor velocity.
	private Vector2 velocity;

	// The direction of input.
	private Vector2 inputDirection;

	// The direction the motor is facing.
	private Vector2 motorDirection;

	private PlayerMotorData motorData;

	private bool shouldApplyAdditiveJump;
	private int additiveJumpAccumulatedVelocity;

	public void Awake() {
		engine = GetComponent<CharacterController2D> ();
		motorData = ScriptableObject.CreateInstance<PlayerMotorData> ();
	}

	public void Update() {
		Debug.AssertFormat (Mathf.Abs (inputDirection.x) <= 1, "ControlDirection.x magnitude exceeded max");
		Debug.AssertFormat (Mathf.Abs (inputDirection.y) <= 1, "ControlDirection.y magnitude exceeded max");

		if (engine.isGrounded) {
			// Horizontal movement.
			if (Mathf.Abs (inputDirection.x) > 0) {
				// There is some directional input applied.
				velocity.x = inputDirection.x * motorData.velocityHorizontalGroundMax;
			} else {
				// No directional input applied or input cancels itself out.
				velocity.x = 0;
			}

			if (shouldApplyAdditiveJump) {
				shouldApplyAdditiveJump = false;
				additiveJumpAccumulatedVelocity = 0;
			}

			if (inputDirection.y > 0) {
				// Perform jump.
				// TODO: Move to PerformJump function
				if (!shouldApplyAdditiveJump) {
					// Apply a small amount of velocity for the first part of the jump.
					// The rest of the jump velocity will be added as vertical input is applied to the motor in the air.
					velocity.y = motorData.velocityJumpImpulse;
					shouldApplyAdditiveJump = true;
				}
			} else if (inputDirection.y < 0) {
				// TODO: Perform crouch
			} else {
				// Controller is grounded and no vertical control direction applied. Zero out Y velocity.
				velocity.y = 0;
			}
		} else {
			// Motor is not grounded.
			bool isWithinAdditiveJumpLimit = additiveJumpAccumulatedVelocity < motorData.velocityJumpAdditive * motorData.frameLimitJumpAdditive;
			if (shouldApplyAdditiveJump && inputDirection.y > 0 && isWithinAdditiveJumpLimit) {
				velocity.y += motorData.velocityJumpAdditive;
				additiveJumpAccumulatedVelocity += motorData.velocityJumpAdditive;
				velocity.y = Mathf.Clamp (velocity.y, -motorData.velocityJumpMax, motorData.velocityJumpMax);
			}

			// Air directional influence
			velocity.x += inputDirection.x * motorData.accelerationHorizontalAir;

			// Clamp horizontal velocity so it doesn't get out of control.
			velocity.x = Mathf.Clamp (velocity.x, -motorData.velocityHorizontalAirMax, motorData.velocityHorizontalAirMax);

			// Apply gravity
			velocity.y -= motorData.gravity;
		}

		// Set the motor direction based on the velocty.
		// TODO: Can be moved to subroutine
		// Check for nonzero velocity
		if (Mathf.Abs (velocity.x) > 1) {
			// Motor direction should be 1 for positive velocity and 0 for negative velocity.
			motorDirection.x = velocity.x > 0 ? 1 : -1;
		}
		if (Mathf.Abs (velocity.y) > 1) {
			motorDirection.y = velocity.y > 0 ? 1 : -1;
		}

		// Update the controller with the computed velocity.
		engine.move(Time.deltaTime * velocity);

		// Update the motor's velocity reference to the computed velocity.
		velocity = engine.velocity;
//		Debug.LogFormat ("Velocity: {0}", velocity);
	}


	// Input functions
	public void InputRight () {
		inputDirection.x = Mathf.Clamp (inputDirection.x + 1, 0, 1);
	}

	public void InputLeft () {
		inputDirection.x = Mathf.Clamp (inputDirection.x - 1, -1, 0);	
	}

	public void InputUp () {
		inputDirection.y = Mathf.Clamp (inputDirection.y + 1, 0, 1);
	}

	public void InputDown () {
		inputDirection.y = Mathf.Clamp (inputDirection.y - 1, -1, 1);
	}

	public void InputHorizontalNone () {
		inputDirection.x = 0;
	}

	public void InputVerticalNone () {
		inputDirection.y = 0;
	}
}
