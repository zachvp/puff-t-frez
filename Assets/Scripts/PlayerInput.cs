using UnityEngine;

// Represents a snapshot of input in a single frame.
public class InputSnapshot<T> where T : IFactoryInput<T>, new()
{
    public T pressed;
    public T released;

    public InputSnapshot()
	{
        pressed = new T();
        released = new T();
    }

    public InputSnapshot(T oldInput, T newInput)
	{
		pressed = newInput;
		released = newInput.Released(oldInput);
    }
}

public class PlayerInput : IFactoryInput<PlayerInput>
{
	public Direction2D direction;
    public bool jump;
	public bool crouch;

    public PlayerInput() {}

    public PlayerInput(PlayerInput input)
	{
        direction = input.direction;
        jump = input.jump;
		crouch = input.crouch;
    }

    // IFactoryInput
	public PlayerInput Clone()
	{
		return new PlayerInput(this);
	}

	public PlayerInput Released(PlayerInput oldInput)
	{
		var copy = Clone();

		copy.jump = CoreUtilities.GetInputReleased(oldInput.jump, jump);
		copy.crouch = CoreUtilities.GetInputReleased(oldInput.crouch, crouch);
		copy.direction = CoreUtilities.GetInputReleased(oldInput.direction, direction);

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

	public HandGrenadeInput Released(HandGrenadeInput oldInput)
	{
		var copy = Clone();

		copy.direction = CoreUtilities.GetInputReleased(oldInput.direction, direction);
		copy.launch = CoreUtilities.GetInputReleased(oldInput.launch, launch);

		return copy;
	}
}
