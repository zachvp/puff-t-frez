using UnityEngine;

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
}
