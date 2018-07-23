using System;
using UnityEngine;

public enum LogicMode
{
	NONE = 0,
    OR   = 1,
    AND  = 2,
    XOR  = 3
}

[Flags]
public enum Direction2D
{
	NONE       = 0,
	RIGHT      = 1 << 0,
	LEFT       = 1 << 1,
	UP         = 1 << 2,
	DOWN       = 1 << 3,
	HORIZONTAL = RIGHT | LEFT,
	VERTICAL   = UP | DOWN,
	ALL        = RIGHT | LEFT | UP | DOWN
}

// The casts to object in the below code are an unfortunate necessity due to
// C#'s restriction against a where T : Enum constraint. (There are ways around
// this, but they're outside the scope of this simple illustration).
// Courtesy of:
//    https://stackoverflow.com/questions/3261451/using-a-bitmask-in-c-sharp
public static class FlagsHelper
{
	public static bool IsSet<T>(T mask, T flag) where T : struct
    {
        var flagsValue = (int)(object) mask;
        var flagValue = (int)(object) flag;

        return (flagsValue & flagValue) != 0;
    }

	public static bool IsSet<T>(T mask, T flagMask, LogicMode mode) where T : struct
	{
		Debug.AssertFormat(mode == LogicMode.AND ||
		                   mode == LogicMode.OR,
		                   "invalid mode provided");

		var shift = 1;
		var matches = 0;
		var flags = 0;
		var result = false;

		for (var i = 0; i < 8 * sizeof(int); ++i)
		{
			var check = (T)(object) (shift << i);

			if (IsSet(flagMask, check))
			{
				// The flagmask has this bit set
				flags++;

				if (IsSet(mask, check))
				{
					// There was a flag match for the input mask
					matches++;
				}
			}
		}

		switch(mode)
		{
			case LogicMode.AND:
				result = matches == flags;
				break;
			case LogicMode.OR:
				result = matches > 0;
				break;
		}

		return result;
	}

	public static void Set<T>(ref T mask, T flag) where T : struct
    {
		var maskValue = (int)(object) mask;
        int flagValue = (int)(object) flag;

        mask = (T)(object) (maskValue | flagValue);
    }

	public static void Set<T> (ref T mask, T flag, bool isSet) where T : struct
	{
		if (isSet)
		{
			Set(ref mask, flag);
		}
		else
		{
			Unset(ref mask, flag);
		}
	}

	public static void Unset<T>(ref T mask, T flag) where T : struct
    {
		var maskValue = (int)(object) mask;
        var flagValue = (int)(object) flag;

        mask = (T)(object) (maskValue & (~flagValue));
    }
}
