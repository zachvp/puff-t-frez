using UnityEngine;

// Represents a snapshot of input in a single frame.
public class PlayerInputSnapshot {
    public PlayerInput pressed;
    public PlayerInput released;

    public PlayerInputSnapshot() {
        pressed = new PlayerInput();
        released = new PlayerInput();
    }

    public PlayerInputSnapshot(PlayerInput pressedInput, PlayerInput releasedInput) {
        pressed = pressedInput;
        released = releasedInput;
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
