using UnityEngine;
using System.Collections.Generic;

// TODO: Override ToString()
public struct CoreDirection
{
	public Direction2D flags
	{
		get         { return varFlags; }
		private set { varFlags = value; }
	}
	public Vector2 vector { get; private set; }

	private Direction2D varFlags;

	public CoreDirection(CoreDirection d)
	{
		varFlags = d.varFlags;
		vector = Convert(d.varFlags);
	}

	public CoreDirection(Vector2 v)
	{
		vector = v;
		varFlags = Convert(v);
	}

	public CoreDirection(Direction2D f)
    {
		varFlags = f;
		vector = Convert(f);
    }

	public void Update(CoreDirection d)
    {
        Update(d.vector);
    }

	public void Update(Vector2 v)
	{
		vector = v;
		varFlags = Convert(v);
	}
    
	public void Update(Direction2D d, bool isSet)
	{
		FlagsHelper.Set(ref varFlags, d, isSet);
		vector = Convert(varFlags);
	}

	public void Update(Direction2D f)
	{
		varFlags = f;
        vector = Convert(f);
	}

	public override string ToString()
	{
		var r = string.Format("flags: {0}  vector: {1}", flags, vector);

		return r;
	}

	private static Vector2 Convert(Direction2D f)
    {
        var result = Vector2.zero;
        var set = 1;
        var unset = 0;

        if (FlagsHelper.IsSet(f, Direction2D.RIGHT))
        {
            result.x = set;
        }
        if (FlagsHelper.IsSet(f, Direction2D.LEFT))
        {
            result.x = -set;
        }
        if (FlagsHelper.IsSet(f, Direction2D.UP))
        {
            result.y = set;
        }
        if (FlagsHelper.IsSet(f, Direction2D.DOWN))
        {
            result.y = -set;
        }

        if (FlagsHelper.IsSet(f, Direction2D.RIGHT) &&
            FlagsHelper.IsSet(f, Direction2D.LEFT))
        {
            result.x = unset;
        }
        if (FlagsHelper.IsSet(f, Direction2D.UP) &&
            FlagsHelper.IsSet(f, Direction2D.DOWN))
        {
            result.y = unset;
        }

        return result;
    }

	private static Direction2D Convert(Vector2 v)
    {
        var result = Direction2D.NONE;
		var list = new List<Direction2D>()
        {
            v.x > 0 ? Direction2D.RIGHT : Direction2D.NONE,
            v.x < 0 ? Direction2D.LEFT : Direction2D.NONE,

            v.y > 0 ? Direction2D.UP : Direction2D.NONE,
            v.y < 0 ? Direction2D.DOWN : Direction2D.NONE
        };

        foreach (Direction2D d in list)
        {
            FlagsHelper.Set(ref result, d);
        }

        return result;
    }
}

public class CoreInput
{
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

	public CoreDirection GetInputReleased(CoreDirection oldInput, CoreDirection newInput)
	{
		var result = new CoreDirection();
		var prime = GetInputReleased(oldInput.vector, newInput.vector);

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
            result.x = 1;
        }
        if (Mathf.Abs(newInput.y) > 0 && Mathf.Abs(oldInput.y) < 1)
        {
            result.y = 1;
        }

        return result;
    }

	public CoreDirection GetInputPressed(CoreDirection oldInput, CoreDirection newInput)
    {
        var result = new CoreDirection();
		var prime = GetInputPressed(oldInput.vector, newInput.vector);

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

	public void ClearConcurrent()
    {
		if (FlagsHelper.IsSet(direction.flags, Direction2D.HORIZONTAL, LogicMode.AND))
        {
			direction.Update(Direction2D.HORIZONTAL, false);
        }
		if (FlagsHelper.IsSet(direction.flags, Direction2D.VERTICAL, LogicMode.AND))
        {
			direction.Update(Direction2D.VERTICAL, false);
        }
    }
}
