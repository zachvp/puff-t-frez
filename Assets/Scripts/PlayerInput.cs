using UnityEngine;

// Represents a snapshot of input in a single frame.
public class PlayerInputSnapshot {
    public PlayerInput pressedInput;
    public PlayerInput releasedInput;

    public PlayerInputSnapshot(PlayerInput pressed, PlayerInput released) {
        pressedInput = pressed;
        releasedInput = released;
    }
}

public class PlayerInput {
    // Pressed states
    public Vector2 movement;
    public bool jump;

    public PlayerInput() {}

    public PlayerInput(PlayerInput input) {
        movement = input.movement;
        jump = input.jump;
    }
}
