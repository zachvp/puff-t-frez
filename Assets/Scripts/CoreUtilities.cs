using UnityEngine;

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
}
