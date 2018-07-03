using UnityEngine;

public class PlayerCharacterInitializer : MonoBehaviour {
    public InputBuffer inputBuffer { get; private set; }

    public void Awake() {
        inputBuffer = new InputBuffer();
	}

    // Create the input controller and player motor dynamically
    // Need
}
