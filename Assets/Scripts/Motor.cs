using UnityEngine;

public class Motor<T, U> where T : ScriptableObject where U : Entity
{
	public readonly U entity;

	protected readonly Transform root;

	protected Vector3 velocity;
	protected T data;

	private bool isSubscribedToUpdate;

	public Motor(U e, Transform t)
	{
		Debug.Assert(e != null, "entity is null");
		Debug.Assert(t != null, "root is null");
		entity = e;
		root = t;
		data = ScriptableObject.CreateInstance<T>();
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
