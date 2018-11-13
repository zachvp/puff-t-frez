public static class Constants
{
	// TODO: Convert to enum (meh)
	public static class Layers
	{
		public const int OBSTACLE = 1 << 8;
		public const int ENTITY = 1 << 9;
	}

    public static class Thresholds
    {
        public const float DOT_THRESHOLD_MIN = 0.01f;
        public const float DOT_THRESHOLD_MAX = 0.99f;
    }

    public static class Directions
    {
        public static CoreDirection RIGHT = new CoreDirection(Direction2D.RIGHT);
        public static CoreDirection LEFT = new CoreDirection(Direction2D.LEFT);
        public static CoreDirection UP = new CoreDirection(Direction2D.UP);
        public static CoreDirection DOWN = new CoreDirection(Direction2D.DOWN);
    }
}
