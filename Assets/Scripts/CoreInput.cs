using UnityEngine;

public class CoreInput
{
	// TODO: Add event to signal when direction has changed (pass old dir, new dir)
	public CoreDirection direction;

	public CoreInput()
	{
		direction = new CoreDirection();
	}

    // Released
    public Direction2D GetInputReleased(Direction2D oldInput, Direction2D newInput)
    {
        var result = Direction2D.NONE;
        var check = 1;

		for (var i = 0; i < 8 * sizeof(int); ++i)
        {
            var current = (Direction2D)(check << i);
            if (FlagsHelper.IsSet(oldInput, current) &&
                !FlagsHelper.IsSet(newInput, current))
            {
                FlagsHelper.Set(ref result, current);
            }
        }

        return result;
    }

    public bool GetInputReleased(bool oldInput, bool newInput)
    {
        return oldInput && !newInput;
    }

    public Vector2 GetInputReleased(Vector2 oldInput, Vector2 newInput)
    {
        var result = Vector2.zero;

        if (Mathf.Abs(newInput.x) < 1 && Mathf.Abs(oldInput.x) > 0)
        {
            result.x = 1;
        }
        if (Mathf.Abs(newInput.y) < 1 && Mathf.Abs(oldInput.y) > 0)
        {
            result.y = 1;
        }

        return result;
    }

	public CoreDirection GetInputReleased(CoreDirection oldInput, CoreDirection newInput)
	{
		var result = new CoreDirection();
		var prime = GetInputReleased(oldInput.Vector, newInput.Vector);

		result.Update(prime);

		return result;
	}

    // Pressed
    public Direction2D GetInputPressed(Direction2D oldInput, Direction2D newInput)
    {
        var result = Direction2D.NONE;
        var check = 1;

        for (var i = 0; i < 8; ++i)
        {
            var current = (Direction2D)(check << i);
            if (!FlagsHelper.IsSet(oldInput, current) &&
                FlagsHelper.IsSet(newInput, current))
            {
                FlagsHelper.Set(ref result, current);
            }
        }

        return result;
    }

    public bool GetInputPressed(bool oldInput, bool newInput)
    {
        return !oldInput && newInput;
    }

    public Vector2 GetInputPressed(Vector2 oldInput, Vector2 newInput)
    {
        var result = Vector2.zero;

        if (Mathf.Abs(newInput.x) > 0 && Mathf.Abs(oldInput.x) < 1)
        {
			result.x = newInput.x;
        }
        if (Mathf.Abs(newInput.y) > 0 && Mathf.Abs(oldInput.y) < 1)
        {
			result.y = newInput.y;
        }
        
        return result;
    }

	public CoreDirection GetInputPressed(CoreDirection oldInput, CoreDirection newInput)
    {
        var result = new CoreDirection();
		var prime = GetInputPressed(oldInput.Vector, newInput.Vector);

		result.Update(prime);

        return result;
    }

	protected void Construct(CoreInput input)
	{
		direction = input.direction;
	}

	protected void Release(CoreInput oldInput)
	{
		direction = GetInputReleased(oldInput.direction, direction);
	}

	protected void Press(CoreInput oldInput)
	{
		direction = GetInputPressed(oldInput.direction, direction);
	}
}

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
        held = newInput.Clone();
        pressed = newInput.Pressed(oldInput);
        released = newInput.Released(oldInput);
    }
}