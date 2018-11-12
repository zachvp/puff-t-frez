using UnityEngine;
using System.Collections.Generic;
using System;

public class CallbackManager
{
    private SortedDictionary<int, Queue<Callback>> postedCallbacks;
	private HashSet<Guid> idempotentCallbacks;

    public CallbackManager()
	{
        postedCallbacks = new SortedDictionary<int, Queue<Callback>>();
		idempotentCallbacks = new HashSet<Guid>();

        FrameCounter.Instance.OnUpdate += HandleOnUpdate;
    }

	public void HandleOnUpdate(long currentFrame, float deltaTime)
	{
        // Iterate over the posted callbacks and see what needs to be fired.
        foreach (var entry in postedCallbacks)
		{
            var callbackQueue = entry.Value;

            if (entry.Key == currentFrame)
			{
                while (callbackQueue.Count > 0)
				{
                    var callback = callbackQueue.Dequeue();

					callback.Fire();
					idempotentCallbacks.Remove(callback.id);
                }
            }
        }
    }

	public void PostCallback(int delay, EventHandler method)
	{
		PostCallback(delay, new Callback(method));
	}

	public void PostCallback(int delay, Callback callback)
	{
		var fireFrame = FrameCounter.Instance.count + delay;

        // Check for existing entry at this time
		if (postedCallbacks.ContainsKey(fireFrame))
		{
            // Entry exists, add callback to existing queue.
			var queue = postedCallbacks[fireFrame];

            queue.Enqueue(callback);
        }
		else
		{
            // Need to create the queue entry.
            var queue = new Queue<Callback>();

            queue.Enqueue(callback);
			postedCallbacks.Add(fireFrame, queue);
        }
    }

	public void PostIdempotentCallback(int delay, Callback callback)
	{
		if (!idempotentCallbacks.Contains(callback.id))
		{
			PostCallback(delay, callback);
		}
		idempotentCallbacks.Add(callback.id);
	}
}

public class Callback
{
	public Guid id;
	private EventHandler handler;

    public Callback(EventHandler completionHandler)
	{
		id = Guid.NewGuid();
		handler = completionHandler;
    }

    public void Fire()
	{
        Events.Raise(handler);
    }
}