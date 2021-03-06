﻿using UnityEngine;
using System.Collections.Generic;

public class CoreDirection
{
    public Direction2D Flags
    {
        get { return varFlags; }
        private set { varFlags = value; }
    }
    public Vector2 Vector { get; private set; }

    // Field to back the public property since we can't pass properties
    // by reference.
    private Direction2D varFlags;

    public CoreDirection() { }

    public CoreDirection(CoreDirection d)
    {
		Update(d);
    }

    public CoreDirection(Vector2 v)
    {
		Update(v);
    }

    public CoreDirection(Direction2D f)
    {
		Update(f);
    }

    public void Update(CoreDirection d)
    {
        Update(d.Vector);
    }

    public void Update(Vector2 v)
    {
        Vector = v;
        varFlags = Convert(v);
    }

    public void Add(CoreDirection d)
    {
        Update(Flags |= d.Flags);
    }

    public void Add(Direction2D d)
    {
        Update(Flags |= d);
    }

    public void Subtract(Direction2D d)
    {
		Update(d, false);
    }

    public void Update(Direction2D d, bool isSet)
    {
        FlagsHelper.Set(ref varFlags, d, isSet);
        Vector = Convert(varFlags);
    }

    public void Update(Direction2D f)
    {
        varFlags = f;
        Vector = Convert(f);
    }

    public void FlipHorizontal()
	{
		var v = Vector;

		v.x = -v.x;

		Update(v);
	}

    public void FlipVertical()
	{
		var v = Vector;

		v.y = -v.y;

		Update(v);
	}

    public void Clear()
    {
        Update(Direction2D.NONE);
    }

	public void ClearConcurrent()
    {
        if (IsSimultaneousHorizontal())
        {
            ClearHorizontal();
        }
        if (IsSimultaneousVertical())
        {
            ClearVertical();
        }
    }

    public void ClearHorizontal()
    {
        Update(Direction2D.HORIZONTAL, false);
    }

    public void ClearVertical()
    {
        Update(Direction2D.VERTICAL, false);
    }

	public void CardinalizeVector()
    {
        Update(Flags);
    }

    public bool IsEmpty()
    {
        return Flags == Direction2D.NONE;
    }

    public bool IsEmptyHorizontal()
	{
		return (int) Vector.x == 0;
	}

    public bool IsEmptyVertical()
	{
		return (int) Vector.y == 0;
	}

    public bool IsSimultaneousHorizontal()
    {
        return FlagsHelper.IsSet(Flags, Direction2D.HORIZONTAL, Logical.AND);
    }

    public bool IsSimultaneousVertical()
    {
        return FlagsHelper.IsSet(Flags, Direction2D.VERTICAL, Logical.AND);
    }

    // Public static
    public static bool IsOppositeHorizontal(CoreDirection lhs, CoreDirection rhs)
    {
        return (int) lhs.Vector.x + (int) rhs.Vector.x == 0;
    }

    public static bool IsOppositeVertical(CoreDirection lhs, CoreDirection rhs)
    {
        return (int) lhs.Vector.y + (int) rhs.Vector.y == 0;
    }

    public static bool IsSameHorizontal(CoreDirection lhs, CoreDirection rhs)
    {
        return (int) lhs.Vector.x == (int) rhs.Vector.x;
    }

    public static bool IsSameVertical(CoreDirection lhs, CoreDirection rhs)
    {
        return (int) lhs.Vector.y == (int) rhs.Vector.y;
    }

    // Overrides
    public override string ToString()
    {
        var r = string.Format("flags: {0}  vector: {1}", Flags, Vector);

        return r;
    }

    // Private
    private Vector2 Convert(Direction2D f)
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

    private Direction2D Convert(Vector2 v)
    {
        var result = Direction2D.NONE;
		var fallback = Direction2D.NONE;
        var list = new List<Direction2D>()
        {
			v.x > 0 ? Direction2D.RIGHT : fallback,
			v.x < 0 ? Direction2D.LEFT : fallback,

			v.y > 0 ? Direction2D.UP : fallback,
			v.y < 0 ? Direction2D.DOWN : fallback
        };

        foreach (Direction2D d in list)
        {
            FlagsHelper.Set(ref result, d);
        }

        return result;
    }
}