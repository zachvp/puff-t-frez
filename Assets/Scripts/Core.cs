using UnityEngine;

public class CoreTransform
{
    public Vector3 position;
    public Vector3 localScale;
    public Quaternion rotation;
    public Transform parent;

    public CoreTransform() { }

    public CoreTransform(Transform t)
    {
        position = t.position;
        localScale = t.localScale;
        rotation = t.rotation;
        parent = t.parent;
    }
}
