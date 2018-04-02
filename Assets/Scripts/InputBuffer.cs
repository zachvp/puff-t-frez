using UnityEngine;
using System.Collections.Generic;

public class InputBuffer {
	private List<Vector2> inputDirectionBuffer;

	public InputBuffer() {
		inputDirectionBuffer = new List<Vector2> ();
	}

	public void AddInput(Vector2 input) {
		inputDirectionBuffer.Add (input);
	}

	public bool isInputReleased(Vector2 checkInputDirection, int checkDimension, int frameWindow) {
		var startIndex = Mathf.Max (0, inputDirectionBuffer.Count - (1 + frameWindow));
		var windowLength = Mathf.Min (frameWindow, inputDirectionBuffer.Count);
		var window = inputDirectionBuffer.GetRange (startIndex, windowLength);
		var pressedCount = 0;
		var isReleased = false;

		// Iterate through the window, checking if given input direction was pressed, then released.
		foreach (Vector2 input in window) {
			if (checkDimension == 0) {
				if (input.x == checkInputDirection.x) {
					pressedCount++;
					isReleased = false;
				} else if (checkInputDirection.x == 0) {
					isReleased = true;
					break;
				}
			} else {
				if (input.y == checkInputDirection.y) {
					pressedCount++;
				} else if (checkInputDirection.y == 0) {
					isReleased = true;
					break;
				}
			}
		}

		return isReleased;
	}
}
