using UnityEngine;

public class PlayerCharacterInitializer : MonoBehaviour {
    public EventHandler<PlayerCharacterInitializer> OnCreate;

    public InputBuffer inputBuffer { get; private set; }

    public void Awake() {
        inputBuffer = new InputBuffer();
	}

	public void Start() {
        // Raise the create event in start so every entity that needs
        // a reference to this can subscribe to it in Awake().
        Events.RaiseEvent(OnCreate, this);
	}
}
