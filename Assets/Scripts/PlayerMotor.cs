using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(CharacterController2D))]
public class PlayerMotor : MonoBehaviour
{
	[SerializeField]
	private CharacterController2D controller;

	public AnimationCurve accelerationCurve;

	private Vector2 velocity;

	private Vector2 controlDirection;

	private Vector2 motorDirection;

	private Stack<Vector2> controlDirections;

	public void Awake() {
		controller = GetComponent<CharacterController2D> ();
		controlDirections = new Stack<Vector2> ();
	}

	public void Update() {
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

		if (isNoHorizontalInput || isConcurrentHorizontalInput)
		{
			InputHorizontalNone ();
		}
		if ((!Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow)))
		{
			InputVerticalNone ();
		}

		controlDirections.Push (controlDirection);

		Debug.Assert (controlDirection.magnitude <= Mathf.Sqrt (2), "Magnitude of control direction exceeded maximum");
//		CoreDebug.LogCollection ("ControlDirections", controlDirections);

		if (controller.isGrounded) {
			// Horizontal movement
			if (Mathf.Abs(velocity.x) < 220)
			{
				velocity.x += controlDirection.x * 60;
			}

			// Horizontal deceleration
			if (Mathf.Abs (velocity.x) < 25) {
				velocity.x = 0;
			} else {
				velocity.x -= 30 * Mathf.Sign (velocity.x);
			}

			if (controlDirection.y > 0) {
				// TODO: Move to InputJump function
				velocity.y = 1200;
				velocity.x += controlDirection.x * 160;
			} else {
				velocity.y = 0;
			}
		} else {
			// Air directional influence
			if (Mathf.Abs(velocity.x) < 220)
			{
//				Debug.LogFormat ("Applying air directional influence");
				velocity.x += controlDirection.x * 60;
//				if (isHorizontalDirectionChanged ()) {
//					Debug.LogFormat ("Horizontal direction changed");
//					velocity.x += controlDirection.x * 120;
//				}
//				Debug.LogFormat ("Apply air directional influence. Velocity: {0}", velocity);
			}

			// Apply gravity
			velocity.y -= 60; // TODO: Make this a constant
		}

		// Apply limits
		if (controller.isGrounded) {
			velocity.x = Mathf.Clamp (velocity.x, -500, 500);
		} else {
			velocity.x = Mathf.Clamp (velocity.x, -2000, 2000);
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
		controller.move(Time.deltaTime * velocity);

		// Update the motor's velocity reference.
		velocity = controller.velocity;

//		Debug.LogFormat ("Velocity: {0}", velocity);
	}

	// Input functions
	public void InputRight () {
		controlDirection.x = Mathf.Clamp (controlDirection.x + 1, 0, 1);
	}

	public void InputLeft () {
		controlDirection.x = Mathf.Clamp (controlDirection.x - 1, -1, 0);	
	}

	public void InputUp () {
		controlDirection.y = Mathf.Clamp (controlDirection.y + 1, 0, 1);
	}

	public void InputDown () {
		controlDirection.y = Mathf.Clamp (controlDirection.y - 1, -1, 1);
	}

	public void InputHorizontalNone () {
		controlDirection.x = 0;
	}

	public void InputVerticalNone () {
		controlDirection.y = 0;
	}

	private bool isHorizontalControlDirectionFlippedInFrameWindow (int frameWindowSize) {
		if (controlDirections.Count > frameWindowSize) {
			List<Vector2> window = new List<Vector2> ();

			// Build a window of the last n frames
			for (int i = 0; i < frameWindowSize; ++i) {
				window.Add (controlDirections.Pop ());
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
