using UnityEngine;

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

public class HandInput : CoreInput, IFactoryInput<HandInput>
{
	public HandInput()
	{
		Construct(this);
	}

	public HandInput(HandInput input)
	{
		Construct(input);
	}

	// IFactoryInput
	public HandInput Clone()
	{
		return new HandInput(this);
	}

	public HandInput Released(HandInput oldInput)
	{
		var c = Clone();

		c.Release(oldInput);

		return c;
	}

	public HandInput Pressed(HandInput oldInput)
	{
		var c = Clone();

		c.Press(oldInput);

        return c;
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
		var c = Clone();

		c.Release(oldInput);
		c.direction = GetInputReleased(oldInput.direction, direction);
		c.launch = GetInputReleased(oldInput.launch, launch);

		return c;
	}

	public HandGrenadeInput Pressed(HandGrenadeInput oldInput)
	{
		var c = Clone();

		c.Press(oldInput);
		c.direction = GetInputPressed(oldInput.direction, direction);
		c.launch = GetInputPressed(oldInput.launch, launch);

		return c;
	}
}
