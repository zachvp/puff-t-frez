using UnityEngine;

// Represents a snapshot of input in a single frame.
public class PlayerInputSnapshot {
    public PlayerInput pressedInput;
    public PlayerInput releasedInput;
}

public class PlayerInput {
    // Pressed states
    public Vector2 movement;
    public bool jump;
}
