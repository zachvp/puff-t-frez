using UnityEngine;
using System;
using System.Collections.Generic;

public class InputBuffer {
    public List<PlayerInputSnapshot> inputBuffer { get; private set; }

	public InputBuffer() {
        inputBuffer = new List<PlayerInputSnapshot> ();
	}

    public void AddInput(PlayerInputSnapshot input) {
		inputBuffer.Add (input);
	}

	// TODO: Implement this so there isn't so much duplicate code =\
	public bool IsInputYReleased(Vector2 checkInputDirection, int frameWindow) {
        return IsInputReleased (checkInputDirection, frameWindow).y == 1;
	}

	public bool IsInputXReleased(Vector2 checkInputDirection, int frameWindow) {
        return IsInputReleased(checkInputDirection, frameWindow).x == 1;
	}

	/// <summary>
	/// Determines whether the given inputMagnitude has flipped from previous frames' magnitude.
	/// </summary>

    // TODO: Fix dis boii
	//public bool IsInputXFlipped(int inputMagnitude, int frameWindow) {
	//	var window = GetFrameWindow (frameWindow);
	//	var isFlipped = false;

 //       foreach (PlayerInputSnapshot input in window) {
	//		if (Mathf.Abs(input.x) > 0 && (int) input.x == -inputMagnitude) {
	//			isFlipped = true;
	//			break;
	//		}
	//	}

	//	return isFlipped;
	//}

    // Given an input, determines which axes were released in the last
    // windowLength frames.
    // TODO: Return some kind of bool Tuple
    private Vector2 IsInputReleased(Vector2 checkInput, int windowLength) {
        // TODO: FIx this
        Debug.LogError("The function IsInputReleased is not fully implemented. Don't call it.");
		var window = GetFrameWindow (windowLength);
        var wasPressed = Vector2.zero;
        var result = Vector2.zero;

		// Iterate through the window, checking if given input direction was pressed, then released.
        foreach (PlayerInputSnapshot input in window) {
            
		}

        return result;
	}

	/// <summary>
	/// Returns a list of the most recent frames with a given window size.
	/// </summary>
	/// <returns>The frame window.</returns>
	/// <param name="frameWindow">Frame window.</param>
    private List<PlayerInputSnapshot> GetFrameWindow(int frameWindow) {
		var startIndex = Mathf.Max (0, inputBuffer.Count - (1 + frameWindow));
		var windowLength = Mathf.Min (frameWindow, inputBuffer.Count);
		var window = inputBuffer.GetRange (startIndex, windowLength);

		return window;
	}
}
