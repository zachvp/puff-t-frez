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
}
