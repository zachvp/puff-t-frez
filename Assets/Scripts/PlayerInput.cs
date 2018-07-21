using UnityEngine;

// Represents a snapshot of input in a single frame.
public class InputSnapshot<T> where T : new()
{
    public T pressed;
    public T released;

    public InputSnapshot()
	{
        pressed = new T();
        released = new T();
    }

    public InputSnapshot(T pressedInput, T releasedInput)
	{
        pressed = pressedInput;
        released = releasedInput;
    }
}

public class PlayerInput : IFactory<PlayerInput>
{
    // Pressed states
    public Vector2 movement;
	// TODO: Could bubble up player state to this class and use input mask
    public bool jump;
	public bool crouch;

    public PlayerInput() {}

    public PlayerInput(PlayerInput input)
	{
        movement = input.movement;
        jump = input.jump;
		crouch = input.crouch;
    }

    // IFactory
	public PlayerInput Clone()
	{
		return new PlayerInput(this);
	}
}

public class HandGrenadeInput : IFactory<HandGrenadeInput>
{
	public Vector2 direction;
	public bool fired;

	public HandGrenadeInput() {}

	public HandGrenadeInput(HandGrenadeInput input)
	{
		direction = input.direction;
		fired = input.fired;
	}

    // IFactory
	public HandGrenadeInput Clone()
	{
		return new HandGrenadeInput(this);
	}
}
