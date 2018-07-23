using UnityEngine;

public class Motor<T, U> where T : ScriptableObject where U : Entity
{
	public readonly U entity;

	protected readonly Transform root;
	protected CoreDirection direction;

	protected Vector3 velocity;
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

	protected void HandleFrameUpdate(EventHandler<long, float> handler)
	{
		if (!isSubscribedToUpdate)
        {
            isSubscribedToUpdate = true;
			FrameCounter.Instance.OnUpdate += handler;
        }
	}

	protected void ClearFrameUpdate(EventHandler<long, float> handler)
	{
		if (isSubscribedToUpdate)
        {
            isSubscribedToUpdate = false;
			FrameCounter.Instance.OnUpdate -= handler;
        }
	}
}
