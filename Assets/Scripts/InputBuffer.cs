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

	/// <summary>
	/// Determines whether the given inputMagnitude has flipped from previous frames' magnitude.
	/// </summary>
	public bool IsInputXFlipped(int inputMagnitude, int frameWindow) {
		var window = GetFrameWindow (frameWindow);
		var isFlipped = false;

		foreach (Vector2 input in window) {
			if (Mathf.Abs(input.x) > 0 && (int) input.x == -inputMagnitude) {
				isFlipped = true;
				break;
			}
		}

		return isFlipped;
	}

	private bool IsInputReleased(int inputMagnitude, int bufferCount, int frameWindow) {
		var window = GetFrameWindow (frameWindow);
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

	/// <summary>
	/// Returns a list of the most recent frames with a given window size.
	/// </summary>
	/// <returns>The frame window.</returns>
	/// <param name="frameWindow">Frame window.</param>
	private List<Vector2> GetFrameWindow(int frameWindow) {
		var startIndex = Mathf.Max (0, inputBuffer.Count - (1 + frameWindow));
		var windowLength = Mathf.Min (frameWindow, inputBuffer.Count);
		var window = inputBuffer.GetRange (startIndex, windowLength);

		return window;
	}
}
