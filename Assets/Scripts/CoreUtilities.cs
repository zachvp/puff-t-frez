using UnityEngine;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System;

public static class CoreUtilities {
	public static Vector3 NormalizePosition(Vector3 position) {
		var prime = Vector3.zero;

		prime.x = Mathf.RoundToInt(position.x);
		prime.y = Mathf.RoundToInt(position.y);
		prime.z = Mathf.RoundToInt(position.z);

		return prime;
	}

	public static Vector3 GetWorldSpaceSize(Vector2 bounds, BoxCollider2D collider, float multiplier)
    {
        var result = Vector3.zero;

        result.x = bounds.x * collider.size.x;
        result.y = bounds.y * collider.size.y;

        return result * multiplier;
    }

	public static Direction2D GetInputReleased(Direction2D oldInput, Direction2D newInput)
	{
		var result = Direction2D.NONE;
		var check = 1;

		for (var i = 0; i < 8; ++i)
		{
			var current = (Direction2D) (check << i);
			if (FlagsHelper.IsSet(oldInput, current) &&
			    !FlagsHelper.IsSet(newInput, current))
			{
				FlagsHelper.Set(ref result, current);
			}
		}

		return result;
	}

	public static bool GetInputReleased(bool oldValue, bool newValue)
	{
		return oldValue && !newValue;
	}

	public static Vector2 GetInputReleased(Vector2 oldInput, Vector2 newInput)
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
}
