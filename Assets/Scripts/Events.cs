public delegate void EventHandler();
public delegate void EventHandler<T>(T args);

public static class Events {
    public static void RaiseEvent(EventHandler eventHandler) {
        // Temp variable for thread safety.
        var threadsafeHandler = eventHandler;
        if (threadsafeHandler != null) {
            threadsafeHandler();
        }
    }

    public static void RaiseEvent<T>(EventHandler<T> eventHandler, T args) {
        // Temp variable for thread safety.
        var threadsafeHandler = eventHandler;
        if (threadsafeHandler != null)
        {
            threadsafeHandler(args);
        }
    }
}
