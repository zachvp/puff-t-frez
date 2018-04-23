using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerKeyboardInputController : MonoBehaviour {
	private IPlayerInput player;

	public void Awake() {
		player = GetComponent<PlayerMotor> ();
	}

	public void Update () {
        // Horizontal control
        Vector2 input = Vector2.zero;

		if (Input.GetKey (KeyCode.RightArrow)) {
			//player.InputRight ();
            input.x += 1;
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			//player.InputLeft ();
            input.x -= 1;
		}

		// Vertical control
		if (Input.GetKey (KeyCode.UpArrow)) {
			//player.InputUp ();
            input.y += 1;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			//player.InputDown ();
            input.y -= 1;
		}

		// Check if the input direction should be neutralized
		var isNoHorizontalInput = !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.LeftArrow);
		var isConcurrentHorizontalInput = Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.LeftArrow);

        var isNoVerticalInput = !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow);
        var isConcurrentVerticalInput = Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.DownArrow);

		if (isNoHorizontalInput || isConcurrentHorizontalInput) {
			//player.InputHorizontalNone ();
            input.x = 0;
		}
        if (isNoVerticalInput || isConcurrentVerticalInput) {
			//player.InputVerticalNone ();
            input.y = 0;
		}

        input.x = Mathf.Clamp(input.x, -1, 1);
        input.y = Mathf.Clamp(input.y, -1, 1);

        player.ApplyInput(input);
        player.ApplyDeltaTime(FrameCounter.Instance.deltaTime);

        Debug.AssertFormat(Mathf.Abs(input.x) <= 1, "Input exceeded bounds: {0}", input);
        Debug.AssertFormat(Mathf.Abs(input.y) <= 1, "Input exceeded bounds: {0}", input);
	}
}
