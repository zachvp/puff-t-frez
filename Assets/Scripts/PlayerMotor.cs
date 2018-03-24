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

	private Stack<Vector2> controlDirections;

	public void Awake() {
		controller = GetComponent<CharacterController2D> ();
		controlDirections = new Stack<Vector2> ();
	}

	public void Update() {
		// Horizontal control
		Debug.LogFormat ("Frame number: {0}", FrameCounter.Instance.count);

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
				// TODO: Move to ApplyJump function
				velocity.y = 1200;
				velocity.x += controlDirection.x * 160;
			}
		} else {
			// Air directional influence
			if (Mathf.Abs(velocity.x) < 220)
			{
				Debug.LogFormat ("Applying air directional influence");
				velocity.x += controlDirection.x * 60;
				if (isHorizontalDirectionChanged ()) {
					Debug.LogFormat ("Horizontal direction changed");
					velocity.x += controlDirection.x * 120;
				}
//				Debug.LogFormat ("Apply air directional influence. Velocity: {0}", velocity);
			}

			// Apply gravity
			velocity.y -= 60; // TODO: Make this a constant
		}

		if (!controller.isGrounded) {
			Debug.LogFormat ("Player is NOT grounded");
		}

		// Apply limits
		if (controller.isGrounded) {
			velocity.x = Mathf.Clamp (velocity.x, -500, 500);
		} else {
			velocity.x = Mathf.Clamp (velocity.x, -2000, 2000);
		}

		controller.move(Time.deltaTime * velocity);
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

	private Boolean isHorizontalDirectionChanged () {
		if (controlDirections.Count > 1) {
			var controlStateCurrent = controlDirections.Pop ();
			var controlStatePrevious = controlDirections.Pop ();

			controlDirections.Push (controlStatePrevious);
			controlDirections.Push (controlStateCurrent);

			return controlStateCurrent.x != controlStatePrevious.x;
		}

		return false;
	}
}
