using UnityEngine;

public class Motor<T, U> where T : ScriptableObject where U : Entity
{
	public readonly U entity;
	public Transform root { get; protected set; }

	protected CoreDirection direction;

	protected T data;

	private bool isSubscribedToUpdate;

	public Motor(U e, Transform t)
	{
		direction = new CoreDirection();
        data = ScriptableObject.CreateInstance<T>();

		entity = e;
		root = t;

		Debug.Assert(e != null, "entity is null");
        Debug.Assert(t != null, "root is null");
	}

	public void SetRoot(Transform t)
	{
		root = t;
	}

    protected void ComputeDirection(Vector3 v)
    {
        var result = direction.Vector;

        // Set the motor direction based on the velocty.
        // Motor direction should be 1 for positive velocity and -1 for
        // negative velocity.
        // Check for nonzero velocity
        if (Mathf.Abs(v.x) > 1)
        {
            result.x = v.x > 0 ? 1 : -1;
        }
        if (Mathf.Abs(v.y) > 1)
        {
            result.y = v.y > 0 ? 1 : -1;
        }

        direction.Update(result);

        Debug.AssertFormat((int)Mathf.Abs(result.x) == 1 ||
                           (int)Mathf.Abs(result.x) == 0,
                           "Motor X direction should always have a magnitude of one.");
        Debug.AssertFormat((int)Mathf.Abs(result.y) == 1 ||
                           (int)Mathf.Abs(result.y) == 0,
                           "Motor Y direction should always have a magnitude of one.");
    }
}
