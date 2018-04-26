using UnityEngine;

[RequireComponent(typeof(PlayerMotor),
                  typeof(PlayerCharacterInitializer))]
public class PlayerKeyboardInputController : MonoBehaviour {
	private IPlayerInput player;
    private PlayerInput input;
    private InputBuffer buffer;

	public void Awake() {
        var initializer = GetComponent<PlayerCharacterInitializer>();

        input = new PlayerInput();
		player = GetComponent<PlayerMotor> ();

        initializer.OnCreate += HandleCreate;
	}

    public void HandleCreate(PlayerCharacterInitializer initializer) {
        buffer = initializer.inputBuffer;
    }

	public void Update () {
        var inputRelease = new PlayerInput();
        var lastInput = new PlayerInput(input);

        input = new PlayerInput();

        // Horizontal control
		if (Input.GetKey (KeyCode.RightArrow)) {
            input.movement.x = 1;
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
            input.movement.x = -1;
		}

		// Vertical control
		if (Input.GetKey (KeyCode.UpArrow)) {
            input.movement.y = 1;
            input.jump = true;
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

        player.ApplyInput(input);
        player.ApplyInputRelease(inputRelease);
        player.ApplyDeltaTime(FrameCounter.Instance.deltaTime);

        // TODO: This should live in a parent input controller class.
        var snapshot = new PlayerInputSnapshot(input, inputRelease);

        buffer.AddInput(snapshot);

        Debug.AssertFormat(Mathf.Abs(input.movement.x) <= 1, "Input exceeded bounds: {0}", input);
        Debug.AssertFormat(Mathf.Abs(input.movement.y) <= 1, "Input exceeded bounds: {0}", input);
	}
}
