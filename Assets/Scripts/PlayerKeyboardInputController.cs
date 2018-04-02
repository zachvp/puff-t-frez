using UnityEngine;

public class PlayerKeyboardInputController : MonoBehaviour {
	private IPlayerInput player;

	public void Awake() {
		player = GetComponent<PlayerMotor> ();
	}

	public void Update () {
		// Horizontal control
		if (Input.GetKey (KeyCode.RightArrow)) {
			player.InputRight ();
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			player.InputLeft ();
		}

		// Vertical control
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			player.InputUp ();
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			player.InputDown ();
		}

		// Check if the input direction should be neutralized
		bool isNoHorizontalInput = !Input.GetKey (KeyCode.RightArrow) && !Input.GetKey (KeyCode.LeftArrow);
		bool isConcurrentHorizontalInput = Input.GetKey (KeyCode.RightArrow) && Input.GetKey (KeyCode.LeftArrow);

		if (isNoHorizontalInput || isConcurrentHorizontalInput) {
			player.InputHorizontalNone ();
		}
		if ((!Input.GetKey (KeyCode.UpArrow) && !Input.GetKey (KeyCode.DownArrow))) {
			player.InputVerticalNone ();
		}
	}
}
