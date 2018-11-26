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

public class HandGrenadeInput : CoreInput, IFactoryInput<HandGrenadeInput>
{
	public bool launch;

	public HandGrenadeInput()
	{
		Construct(this);
	}

	public HandGrenadeInput(HandGrenadeInput input)
	{
		Construct(input);
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

		c.Release(oldInput);
		c.launch = GetInputReleased(oldInput.launch, launch);

		return c;
	}

	public HandGrenadeInput Pressed(HandGrenadeInput oldInput)
	{
		var c = Clone();

		c.Press(oldInput);
		c.launch = GetInputPressed(oldInput.launch, launch);

		return c;
	}
}

public class CombatHandInput : CoreInput, IFactoryInput<CombatHandInput>
{
	public bool grab;

	public CombatHandInput()
	{
		Construct(this);
	}

	public CombatHandInput(CombatHandInput input)
	{
		Construct(input);
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

		c.Release(oldInput);
		c.grab = GetInputReleased(oldInput.grab, grab);

		return c;
	}

	public CombatHandInput Pressed(CombatHandInput oldInput)
	{
		var c = Clone();

		c.Press(oldInput);
		c.grab = GetInputPressed(oldInput.grab, grab);

		return c;
	}
}
