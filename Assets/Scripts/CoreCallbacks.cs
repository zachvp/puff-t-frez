using System.Collections.Generic;

// DBG Only?
using UnityEngine;

public class CallbackManager {
    private SortedDictionary<int, Queue<Callback>> postedCallbacks;

    public CallbackManager() {
        postedCallbacks = new SortedDictionary<int, Queue<Callback>>();

        FrameCounter.Instance.OnUpdate += HandleOnUpdate;
    }

    public void HandleOnUpdate(int currentFrame) {
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
		Debug.LogFormat("Callback posted! Frame: {0}", FrameCounter.Instance.count);
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
		Debug.LogFormat("Callback fired! Frame: {0}", FrameCounter.Instance.count);
        Events.Raise(handler);
    }
}