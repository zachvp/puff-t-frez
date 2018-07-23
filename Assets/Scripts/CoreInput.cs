using UnityEngine;

public class CoreInput
{
	public Direction2D direction;

    // Released
    public Direction2D GetInputReleased(Direction2D oldInput, Direction2D newInput)
    {
        var result = Direction2D.NONE;
        var check = 1;

        for (var i = 0; i < 8; ++i)
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
            result.x = 1;
        }
        if (Mathf.Abs(newInput.y) > 0 && Mathf.Abs(oldInput.y) < 1)
        {
            result.y = 1;
        }

        return result;
    }

	protected void Release(CoreInput oldInput)
	{
		direction = GetInputReleased(oldInput.direction, direction);
	}

	protected void Press(CoreInput oldInput)
	{
		direction = GetInputPressed(oldInput.direction, direction);
	}

	public void ClearConcurrent()
    {
		if (FlagsHelper.IsSet(direction, Direction2D.HORIZONTAL, LogicMode.AND))
        {
			FlagsHelper.Unset(ref direction, Direction2D.HORIZONTAL);
        }
		if (FlagsHelper.IsSet(direction, Direction2D.VERTICAL, LogicMode.AND))
        {
			FlagsHelper.Unset(ref direction, Direction2D.VERTICAL);
        }
    }
}
