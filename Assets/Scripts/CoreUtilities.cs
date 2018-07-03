using UnityEngine;

public static class CoreUtilities {
	public static Vector3 NormalizePosition(Vector3 position) {
		var prime = Vector3.zero;

		prime.x = Mathf.RoundToInt(position.x);
		prime.y = Mathf.RoundToInt(position.y);
		prime.z = Mathf.RoundToInt(position.z);

		return prime;
	}
}
