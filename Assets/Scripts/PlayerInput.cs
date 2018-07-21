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

public class PlayerInput : IFactoryInput<PlayerInput>
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

	// TODO: Implement
	public PlayerInput Released(PlayerInput oldInput)
	{
		var copy = Clone();

		copy.jump = !jump;
		copy.crouch = !crouch;
		copy.movement = -movement;

		return copy;
	}
}

public class HandGrenadeInput : IFactoryInput<HandGrenadeInput>
{
	public Direction2D direction;
	public bool launch;

	public HandGrenadeInput() {}

	public HandGrenadeInput(HandGrenadeInput input)
	{
		direction = input.direction;
		launch = input.launch;
	}

    // IFactory
	public HandGrenadeInput Clone()
	{
		return new HandGrenadeInput(this);
	}

	// TODO: Replace with released form given other input
	public HandGrenadeInput Released(HandGrenadeInput oldInput)
	{
		var copy = Clone();

		copy.direction = ~direction;
		launch = !launch;

		return copy;
	}
}
