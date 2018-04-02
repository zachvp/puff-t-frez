using UnityEngine;
using System.Collections.Generic;

public class InputBuffer {
	private List<Vector2> inputBuffer;

	public InputBuffer() {
		inputBuffer = new List<Vector2> ();
	}

	public void AddInput(Vector2 input) {
		inputBuffer.Add (input);
	}

	// TODO: Implement this so there isn't so much duplicate code =\
	public bool IsInputYReleased(Vector2 checkInputDirection, int frameWindow) {
		return IsInputReleased ((int) checkInputDirection.y, inputBuffer.Count, frameWindow);
	}

	public bool IsInputXReleased(Vector2 checkInputDirection, int frameWindow) {
		return IsInputReleased ((int) checkInputDirection.x, inputBuffer.Count, frameWindow);
	}

	private bool IsInputReleased(int inputMagnitude, int bufferCount, int frameWindow) {
		var startIndex = Mathf.Max (0, bufferCount - (1 + frameWindow));
		var windowLength = Mathf.Min (frameWindow, bufferCount);
		var window = inputBuffer.GetRange (startIndex, windowLength);
		var isReleased = false;

		// Iterate through the window, checking if given input direction was pressed, then released.
		foreach (Vector2 input in window) {
			if (inputMagnitude == 0) {
				isReleased = true;
				break;
			}
		}

		return isReleased;
	}
}
