using UnityEngine;

public static class CoreUtilities
{
	public static Vector3 NormalizePosition(Vector3 position)
	{
		var r = Vector3.zero;

		r.x = Mathf.RoundToInt(position.x);
		r.y = Mathf.RoundToInt(position.y);
		r.z = Mathf.RoundToInt(position.z);

		return r;
	}

	public static Vector2 NormalizeScale(Vector3 scale)
	{
		var r = Vector3.zero;
		var digits = 3;
		var power = 10 * digits;

		r.x = Mathf.Round(scale.x * power) / power;
		r.y = Mathf.Round(scale.y * power) / power;
		r.y = Mathf.Round(scale.z * power) / power;

		return r;
	}

	public static Vector3 GetWorldSpaceSize(Vector2 bounds, BoxCollider2D collider, float multiplier)
    {
		var r = Vector3.zero;

        r.x = bounds.x * collider.size.x;
        r.y = bounds.y * collider.size.y;

        return r * multiplier;
    }
}
