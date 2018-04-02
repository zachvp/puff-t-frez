using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMotor : MonoBehaviour
{
	// Reference to the character controller engine.
	private CharacterController2D engine;

	// The motor velocity.
	private Vector2 velocity;

	// The direction of input.
	private Vector2 inputDirection;

	// The direction the motor is facing.
	private Vector2 motorDirection;

	private Stack<Vector2> inputDirectionBuffer;

	private PlayerMotorData motorData;

	private bool shouldApplyAdditiveJump;
	private int additiveJumpAmount;

	public void Awake() {
		engine = GetComponent<CharacterController2D> ();
		inputDirectionBuffer = new Stack<Vector2> ();
		motorData = ScriptableObject.CreateInstance<PlayerMotorData> ();
	}

	// TODO: Variable jump height based on how long up input is pressed for

	public void Update() {
		UpdateInput ();

		inputDirectionBuffer.Push (inputDirection);

		Debug.AssertFormat (Mathf.Abs (inputDirection.x) <= 1, "ControlDirection.x magnitude exceeded max");
		Debug.AssertFormat (Mathf.Abs (inputDirection.y) <= 1, "ControlDirection.y magnitude exceeded max");

//		CoreDebug.LogCollection ("ControlDirections", controlDirections);

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
				additiveJumpAmount = 0;
			}

			if (inputDirection.y > 0) {
				// Perform jump.
				// TODO: Move to PerformJump function
				if (!shouldApplyAdditiveJump) {
					// Apply a small amount of velocity for the first part of the jump.
					// The rest of the jump velocity will be added as vertical input is applied to the motor in the air.
					velocity.y = 400;
					shouldApplyAdditiveJump = true;
				}
			} else if (inputDirection.y < 0) {
				// TODO: Perform crouch
			} else {
				// Controller is grounded and no vertical control direction applied. Zero out Y velocity.
				velocity.y = 0;
			}
		} else {
			// Not grounded.

			if (shouldApplyAdditiveJump && inputDirection.y > 0 && additiveJumpAmount < 1200) {
				velocity.y += 120;
				additiveJumpAmount += 120;
				velocity.y = Mathf.Clamp (velocity.y, -800, 800);
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
	// TODO: These should be interface-implemented methods.
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

	// TODO: Move this to a separate component.
	private void UpdateInput() {
		// Horizontal control
		if (Input.GetKey (KeyCode.RightArrow)) {
			InputRight ();
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			InputLeft ();
		}

		// Vertical control
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			InputUp ();
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			InputDown ();
		}

		//		Debug.LogFormat ("{0} ControlDirection: {1}", FrameCounter.Instance.count, controlDirection);

		// Check if the input direction should be neutralized
		Boolean isNoHorizontalInput = !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.LeftArrow);
		Boolean isConcurrentHorizontalInput = Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.LeftArrow);

		if (isNoHorizontalInput || isConcurrentHorizontalInput) {
			InputHorizontalNone ();
		}
		if ((!Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow))) {
			InputVerticalNone ();
		}
	}

	private bool isHorizontalControlDirectionFlippedInFrameWindow (int frameWindowSize) {
		if (inputDirectionBuffer.Count > frameWindowSize) {
			List<Vector2> window = new List<Vector2> ();

			// Build a window of the last n frames
			for (int i = 0; i < frameWindowSize; ++i) {
				window.Add (inputDirectionBuffer.Pop ());
			}

//			CoreDebug.LogCollection ("frameWindow", window);
//			Debug.LogFormat ("Frame window: {0}", window.ToString ());

			// Iterate through the window, store first nonzero direction, then see if it's the negative of the next
			// nonzero direction.
			Vector2 initialDirection = Vector2.zero;
			Vector2 finalDirection = Vector2.zero;

			foreach (Vector2 direction in window) {
				if (Mathf.Abs (direction.x) > 0) {
					if (Mathf.Abs (initialDirection.x) > 0) {
						// We have an initial direction, see if the current direction is opposite of that.
						if (initialDirection.x == -direction.x) {
							return true;
						}
					} else {
						// Store the first nonzero direction
						initialDirection = direction;
					}
				}
			}

			// Add directions back to the stack.
//			controlDirections.Push (controlStatePrevious);
//			controlDirections.Push (controlStateCurrent);

//			return controlStateCurrent.x == -controlStatePrevious.x;
		}

		return false;
	}
}
