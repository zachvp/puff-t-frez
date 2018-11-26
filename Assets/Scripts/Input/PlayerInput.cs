public class PlayerInput : CoreInput, IFactoryInput<PlayerInput>
{
	public CoreDirection direction;
    public bool jump;
	public bool crouch;
    
	public PlayerInput()
	{
		direction = new CoreDirection();
	}

	public PlayerInput(PlayerInput input)
	{
		direction = new CoreDirection(input.direction);
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

		copy.direction = GetInputReleased(oldInput.direction, direction);
		copy.jump = GetInputReleased(oldInput.jump, jump);
		copy.crouch = GetInputReleased(oldInput.crouch, crouch);

		return copy;
	}

	public PlayerInput Pressed(PlayerInput oldInput)
	{
		var copy = Clone();

		copy.direction = GetInputPressed(oldInput.direction, direction);
		copy.jump = GetInputPressed(oldInput.jump, jump);
		copy.crouch = GetInputPressed(oldInput.crouch, crouch);

		return copy;
	}
}

public class HandGrenadeInput : CoreInput, IFactoryInput<HandGrenadeInput>
{
	public CoreDirection direction;
	public bool launch;

	public HandGrenadeInput()
	{
		direction = new CoreDirection();
	}

	public HandGrenadeInput(HandGrenadeInput input)
	{
		direction = new CoreDirection(input.direction);
		launch = input.launch;
	}

	// IFactoryInput
	public HandGrenadeInput Clone()
	{
		return new HandGrenadeInput(this);
	}

	public HandGrenadeInput Released(HandGrenadeInput oldInput)
	{
		var c = Clone();

		c.direction = GetInputReleased(oldInput.direction, direction);
		c.launch = GetInputReleased(oldInput.launch, launch);

		return c;
	}

	public HandGrenadeInput Pressed(HandGrenadeInput oldInput)
	{
		var c = Clone();

		c.direction = GetInputPressed(oldInput.direction, direction);
		c.launch = GetInputPressed(oldInput.launch, launch);

		return c;
	}
}

public class CombatHandInput : CoreInput, IFactoryInput<CombatHandInput>
{
	public CoreDirection direction;
	public bool grab;

	public CombatHandInput()
	{
		direction = new CoreDirection();
	}

	public CombatHandInput(CombatHandInput input)
	{
		direction = new CoreDirection(input.direction);
		grab = input.grab;
	}

	// IFactoryInput
	public CombatHandInput Clone()
	{
		return new CombatHandInput(this);
	}

	public CombatHandInput Released(CombatHandInput oldInput)
	{
		var c = Clone();

		c.direction = GetInputReleased(oldInput.direction, direction);
		c.grab = GetInputReleased(oldInput.grab, grab);

		return c;
	}

	public CombatHandInput Pressed(CombatHandInput oldInput)
	{
		var c = Clone();

		c.direction = GetInputPressed(oldInput.direction, direction);
		c.grab = GetInputPressed(oldInput.grab, grab);

		return c;
	}
}
