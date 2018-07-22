using UnityEngine;
using System.Collections.Generic;

public static class CoreUtilities
{
	public static Vector3 NormalizePosition(Vector3 position)
	{
		var prime = Vector3.zero;

		prime.x = Mathf.RoundToInt(position.x);
		prime.y = Mathf.RoundToInt(position.y);
		prime.z = Mathf.RoundToInt(position.z);

		return prime;
	}

	public static Vector2 NormalizeScale(Vector3 scale)
	{
		var result = Vector3.zero;
		var digits = 3;
		var power = 10 * digits;

		result.x = Mathf.Round(scale.x * power) / power;
		result.y = Mathf.Round(scale.y * power) / power;
		result.y = Mathf.Round(scale.z * power) / power;

		return result;
	}

	public static Vector3 GetWorldSpaceSize(Vector2 bounds, BoxCollider2D collider, float multiplier)
    {
        var result = Vector3.zero;

        result.x = bounds.x * collider.size.x;
        result.y = bounds.y * collider.size.y;

        return result * multiplier;
    }
        
	public static Vector2 Convert(Direction2D direction)
	{
		var result = Vector2.zero;
		var set = 1;
		var unset = 0;

		if (FlagsHelper.IsSet(direction, Direction2D.RIGHT))
		{
			result.x = set;
		}
		if (FlagsHelper.IsSet(direction, Direction2D.LEFT))
		{
			result.x = -set;
		}
		if (FlagsHelper.IsSet(direction, Direction2D.UP))
		{
			result.y = set;
		}
		if (FlagsHelper.IsSet(direction, Direction2D.DOWN))
        {
			result.y = -set;
        }

		if (FlagsHelper.IsSet(direction, Direction2D.RIGHT) &&
		    FlagsHelper.IsSet(direction, Direction2D.LEFT))
		{
			result.x = unset;
		}
		if (FlagsHelper.IsSet(direction, Direction2D.UP) &&
		    FlagsHelper.IsSet(direction, Direction2D.DOWN))
        {
            result.y = unset;
        }

		return result;
	}

	public static Direction2D Convert(Vector2 vector)
	{
		var result = Direction2D.NONE;
		var list = new List<Direction2D>()
		{
			vector.x > 0 ? Direction2D.RIGHT : Direction2D.NONE,
			vector.x < 0 ? Direction2D.LEFT : Direction2D.NONE,
			vector.y > 0 ? Direction2D.UP : Direction2D.NONE,
			vector.y < 0 ? Direction2D.DOWN : Direction2D.NONE
		};
                
		foreach (Direction2D direction in list)
		{
			FlagsHelper.Set(ref result, direction);
		}

		return result;
	}

	// TODO: Bool to mask function - returns mask / negated value
}
