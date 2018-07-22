using System;
using UnityEngine;
using System.Runtime.InteropServices;

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

	public static bool IsSet<T>(T mask, T flagMask, bool shouldMatchAll) where T : struct
	{
		var shift = 1;
		var matches = 0;
		var flags = 0;

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

		return shouldMatchAll ? matches == flags : matches > 0;
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
