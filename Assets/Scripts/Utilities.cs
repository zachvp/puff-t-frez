using UnityEngine;

public class StoreTransform {
	public Vector3 position;
	public Vector3 localScale;
	public Quaternion rotation;

	public StoreTransform() {}

	public StoreTransform(Transform t) {
		position = t.position;
		localScale = t.localScale;
		rotation = t.rotation;
	}
}