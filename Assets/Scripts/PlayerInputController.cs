using UnityEngine;

public class PlayerInputController : MonoBehaviour {
	protected IPlayerInput player;

	protected PlayerInput input;

	protected PlayerInput inputRelease;
	protected PlayerInput lastInput;

	protected InputBuffer buffer;

	public void Awake() {
		var initializer = GetComponent<PlayerCharacterInitializer>();

		input = new PlayerInput();
		player = GetComponent<PlayerMotor> ();

		initializer.OnCreate += HandleCreate;
	}

    public void HandleCreate(PlayerCharacterInitializer initializer) {
        buffer = initializer.inputBuffer;
    }

	public void Update() {
		inputRelease = new PlayerInput();
        lastInput = new PlayerInput(input);

        input = new PlayerInput();

		HandleUpdate();
	}

	public virtual void HandleUpdate() {
		// Handled by subclasses
	}

	protected void HandleInputRight() {
		input.movement.x = 1;
	}

	protected void HandleInputLeft() {
		input.movement.x = -1;
	}

	protected void HandleInputUp() {
		input.movement.y = 1;
	}

	protected void HandleInputDown() {
		input.movement.y = -1;
	}

	protected void HandleInputJump() {
		input.jump = true;
	}

	protected void HandleInputChecksFinished() {
		var snapshot = new PlayerInputSnapshot(input, inputRelease);

        player.ApplyInput(snapshot);
        player.ApplyDeltaTime(FrameCounter.Instance.deltaTime);

        buffer.AddInput(snapshot);

        Debug.AssertFormat(Mathf.Abs(input.movement.x) <= 1, "Input exceeded bounds: {0}", input);
        Debug.AssertFormat(Mathf.Abs(input.movement.y) <= 1, "Input exceeded bounds: {0}", input);
	}
}
