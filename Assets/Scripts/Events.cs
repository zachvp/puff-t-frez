public delegate void EventHandler();
public delegate void EventHandler<T>(T arg);
public delegate void EventHandler<T, U>(T arg0, U arg1);

public static class Events {
    public static void Raise(EventHandler eventHandler) {
        // Temp variable for thread safety.
        var threadsafeHandler = eventHandler;
        if (threadsafeHandler != null) {
            threadsafeHandler();
        }
    }

    public static void Raise<T>(EventHandler<T> eventHandler, T arg) {
        // Temp variable for thread safety.
        var threadsafeHandler = eventHandler;
        if (threadsafeHandler != null)
        {
            threadsafeHandler(arg);
        }
    }

	public static void Raise<T, U>(EventHandler<T, U> eventHandler, T arg0, U arg1) {
		var threadSafeHandler = eventHandler;
		if (threadSafeHandler != null) {
			threadSafeHandler(arg0, arg1);
		}
	}
}
