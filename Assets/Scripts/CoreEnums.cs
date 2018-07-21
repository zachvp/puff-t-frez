using System;
using UnityEngine;

[Flags]
public enum Direction2D
{
	NONE  = 0,
	RIGHT = 1 << 0,
	LEFT  = 1 << 1,
	UP    = 1 << 2,
	DOWN  = 1 << 3,
	ALL   = RIGHT | LEFT | UP | DOWN
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

	public static void Set<T>(ref T mask, T flag) where T : struct
    {
        var flagsValue = (int)(object) mask;
        int flagValue = (int)(object) flag;

        mask = (T)(object) (flagsValue | flagValue);
    }

	public static void Unset<T>(ref T mask, T flag) where T : struct
    {
        var flagsValue = (int)(object) mask;
        var flagValue = (int)(object) flag;

        mask = (T)(object) (flagsValue & (~flagValue));
    }

	//public static T GetOverride<T>(T oldMask, T overrideMask) where T : struct
	//{
	//	byte[] oldBytes = BitConverter.GetBytes((int)(object) oldMask);
	//	byte[] overrideBytes = BitConverter.GetBytes((int)(object) overrideMask);
	//	int length = oldBytes.Length; // both lengths are the same since types are the same
 //       int bitPos = 0;
	//	T result = (T) (object) 0;

	//	while (bitPos < 8 * length)
 //       {
 //           int byteIndex = bitPos / 8;
 //           int offset = bitPos % 8;
	//		bool isOldSet = (oldBytes[byteIndex] & (1 << offset)) != 0;
	//		bool isOverrideSet = (overrideBytes[byteIndex] & (1 << offset)) != 0;

	//		if (isOverrideSet)
	//		{
	//		}

 //           // isSet = [True] if the bit at bitPos is set, false otherwise

 //           bitPos++;
 //       }

	//	return result;
	//}
}
