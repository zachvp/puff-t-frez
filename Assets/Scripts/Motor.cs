using UnityEngine;

public class Motor
{
	public Entity entity { get; protected set; }
	protected Vector3 velocity;

	private bool isSubscribedToUpdate;

	protected void HandleFrameUpdate(EventHandler<int, float> handler)
	{
		if (!isSubscribedToUpdate)
        {
            isSubscribedToUpdate = true;
			FrameCounter.Instance.OnUpdate += handler;
        }
	}

	protected void ClearFrameUpdate(EventHandler<int, float> handler)
	{
		if (isSubscribedToUpdate)
        {
            isSubscribedToUpdate = false;
			FrameCounter.Instance.OnUpdate -= handler;
        }
	}
}
