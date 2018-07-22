using System.Collections.Generic;

public class CallbackManager {
    private SortedDictionary<int, Queue<Callback>> postedCallbacks;

    public CallbackManager() {
        postedCallbacks = new SortedDictionary<int, Queue<Callback>>();

        FrameCounter.Instance.OnUpdate += HandleOnUpdate;
    }

	public void HandleOnUpdate(long currentFrame, float deltaTime) {
        // Iterate over the posted callbacks and see what needs to be fired.
        foreach (var entry in postedCallbacks) {
            var callbackQueue = entry.Value;

            if (entry.Key == currentFrame) {
                while (callbackQueue.Count > 0) {
                    var callback = callbackQueue.Dequeue();

					callback.Fire();
                }
            }
        }
    }

	public void PostCallbackWithFrameDelay(int delay, Callback callback) {
		var fireFrame = FrameCounter.Instance.count + delay;

        // Check for existing entry at this time
		if (postedCallbacks.ContainsKey(fireFrame)) {
            // Entry exists, add callback to existing queue.
			var queue = postedCallbacks[fireFrame];

            queue.Enqueue(callback);
        } else {
            // Need to create the queue entry.
            var queue = new Queue<Callback>();

            queue.Enqueue(callback);
			postedCallbacks.Add(fireFrame, queue);
        }
    }
}

public class Callback {
    public EventHandler handler;

    public Callback(EventHandler completionHandler) {       
		handler = completionHandler;
    }

    public void Fire() {
        Events.Raise(handler);
    }
}