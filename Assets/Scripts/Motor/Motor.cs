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
}
