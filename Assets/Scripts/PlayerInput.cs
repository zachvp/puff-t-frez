using UnityEngine;

// Represents a snapshot of input in a single frame.
public class InputSnapshot<T> where T : CoreInput, IFactoryInput<T>, new()
{
    public T pressed;
	public T held;
    public T released;
    
    public InputSnapshot()
	{
        pressed = new T();
		held = new T();
        released = new T();
    }

    public InputSnapshot(T oldInput, T newInput)
	{
		held = newInput;
		pressed  = newInput.Pressed(oldInput);
		released = newInput.Released(oldInput);
    }
}

public class PlayerInput : CoreInput, IFactoryInput<PlayerInput>
{
    public bool jump;
	public bool crouch;
    
	public PlayerInput() { }

	public PlayerInput(PlayerInput input)
	{
		Construct(input);
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

		copy.Release(oldInput);
		copy.jump = GetInputReleased(oldInput.jump, jump);
		copy.crouch = GetInputReleased(oldInput.crouch, crouch);

		return copy;
	}

	public PlayerInput Pressed(PlayerInput oldInput)
	{
		var copy = Clone();

		copy.Press(oldInput);
		copy.jump = GetInputPressed(oldInput.jump, jump);
		copy.crouch = GetInputPressed(oldInput.crouch, crouch);

		return copy;
	}
}

// TODO: Move this class to a different file
public class MotorData
{
	public CoreDirection direction;
	public Vector3 velocity;

	public MotorData()
	{
		direction = new CoreDirection();
	}

	public MotorData(CoreDirection d, Vector3 v)
	{
		direction = new CoreDirection(d);
		velocity = v;
	}

	public MotorData(MotorData other)
	{
		direction = new CoreDirection(other.direction);
		velocity = other.velocity;
	}
}

public class HandGrenadeInput : CoreInput, IFactoryInput<HandGrenadeInput>
{
	public bool launch;
	public MotorData data;

	public HandGrenadeInput()
	{
		data = new MotorData();
		Construct(this);
	}

	public HandGrenadeInput(HandGrenadeInput input)
	{
		if (input.launch)
		{
			var t = 0;
		}

		Construct(input);
		launch = input.launch;
		data = new MotorData(input.data);
	}

	// IFactoryInput
	public HandGrenadeInput Clone()
	{
		return new HandGrenadeInput(this);
	}

	public HandGrenadeInput Released(HandGrenadeInput oldInput)
	{
		var copy = Clone();

		Release(oldInput);
		copy.direction = GetInputReleased(oldInput.direction, direction);
		copy.launch = GetInputReleased(oldInput.launch, launch);

		return copy;
	}

	public HandGrenadeInput Pressed(HandGrenadeInput oldInput)
	{
		var copy = Clone();

		Press(oldInput);
		copy.direction = GetInputPressed(oldInput.direction, direction);
		copy.launch = GetInputPressed(oldInput.launch, launch);

		return copy;
	}
}
