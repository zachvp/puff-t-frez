using System;
using System.Diagnostics;

// TODO: Should Vector2 just be used for this??
[Flags]
public enum Direction2D
{
	RIGHT = 1 << 0,
	LEFT  = 1 << 1,
	ABOVE = 1 << 2,
	BELOW = 1 << 3,
	ALL   = RIGHT | LEFT | ABOVE | BELOW
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
        var flagsValue = (int)(object)mask;
        var flagValue = (int)(object)flag;

        return (flagsValue & flagValue) != 0;
    }

	public static void Set<T>(ref T mask, T flag) where T : struct
    {
        var flagsValue = (int)(object)mask;
        int flagValue = (int)(object)flag;

        mask = (T)(object)(flagsValue | flagValue);
    }

	public static void Unset<T>(ref T mask, T flag) where T : struct
    {
        var flagsValue = (int)(object)mask;
        var flagValue = (int)(object)flag;

        mask = (T)(object)(flagsValue & (~flagValue));
    }
}
