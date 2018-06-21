using UnityEngine;

public class StoreTransform {
	public Vector3 position;
	public Vector3 scale;
	public Quaternion rotation;

	public StoreTransform() {}

	public StoreTransform(Transform t) {
		position = t.position;
		scale = t.localScale;
		rotation = t.rotation;
	}
}