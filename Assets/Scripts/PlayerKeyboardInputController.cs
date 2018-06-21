using UnityEngine;

[RequireComponent(typeof(PlayerMotor),
                  typeof(PlayerCharacterInitializer))]
public class PlayerKeyboardInputController : PlayerInputController {
	public override void HandleUpdate() {
        // Horizontal control
		if (Input.GetKey (KeyCode.RightArrow)) {
            HandleInputRight();
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
            HandleInputLeft();
		}

		// Vertical control
		if (Input.GetKey (KeyCode.UpArrow)) {
            HandleInputUp();
            HandleInputJump();
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
            input.movement.y = -1;
		}

		// Check if the input direction should be neutralized
		var isNoHorizontalInput = !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.LeftArrow);
		var isConcurrentHorizontalInput = Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.LeftArrow);

        var isNoVerticalInput = !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow);
        var isConcurrentVerticalInput = Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow);

		if (isNoHorizontalInput || isConcurrentHorizontalInput) {
            input.movement.x = 0;
		}
        if (isNoVerticalInput || isConcurrentVerticalInput) {
            input.movement.y = 0;
		}

        // Check for releases.
        if (Mathf.Abs(input.movement.x) < 1 && Mathf.Abs(lastInput.movement.x) > 0) {
            inputRelease.movement.x = 1;
        }
        if (Mathf.Abs(input.movement.y) < 1 && Mathf.Abs(lastInput.movement.y) > 0) {
            inputRelease.movement.y = 1;
        }
        if (!input.jump && lastInput.jump) {
            inputRelease.jump = true;
        }

        HandleInputChecksFinished();
	}
}
