using UnityEngine;

public class PlayerInputControllerKeyboard : PlayerInputController {
	public PlayerInputControllerKeyboard(IPlayerInput playerInput, InputBuffer inputBuffer)
		: base(playerInput, inputBuffer)
	{ }

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
            HandleInputDown();
		}

		// Check if the input direction should be neutralized
		var isConcurrentHorizontalInput = Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.LeftArrow);
        var isConcurrentVerticalInput = Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow);

        HandleInputChecksFinished(isConcurrentHorizontalInput, isConcurrentVerticalInput);
	}
}
