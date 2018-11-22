using UnityEngine;

public class CollisionState2D
{
	public CoreDirection direction;
	public bool Left
	{
		get { return FlagsHelper.IsSet(direction.Flags, Direction2D.LEFT); }
		set { SetDirectionForBool(Direction2D.LEFT, value); }
	}
	public bool Right
	{
		get { return FlagsHelper.IsSet(direction.Flags, Direction2D.RIGHT); }
		set { SetDirectionForBool(Direction2D.RIGHT, value); }
	}
	public bool Above
	{
		get { return FlagsHelper.IsSet(direction.Flags, Direction2D.UP); }
		set { SetDirectionForBool(Direction2D.UP, value); }
	}
	public bool Below
	{
		get { return FlagsHelper.IsSet(direction.Flags, Direction2D.DOWN); }
		set { SetDirectionForBool(Direction2D.DOWN, value); }
	}

    public bool becameGroundedThisFrame;
    public bool movingDownSlope;
    public float slopeAngle;

    public CollisionState2D()
    {
        direction = new CoreDirection();
    }

    public CollisionState2D(CollisionState2D other)
    {
        Update(other);
    }

    public CollisionState2D(Collision2D c)
    {
        Update(c);
    }

	public bool HasCollision()
    {
		return FlagsHelper.IsSet(direction.Flags, Direction2D.ALL);
    }

    public void Update(CollisionState2D s)
    {
        direction = s.direction;
        becameGroundedThisFrame = s.becameGroundedThisFrame;
        movingDownSlope = s.movingDownSlope;
        slopeAngle = s.slopeAngle;
    }

    public void Add(CollisionState2D s)
    {
        direction.Add(s.direction);
        becameGroundedThisFrame |= s.becameGroundedThisFrame;
        movingDownSlope |= s.movingDownSlope;
        slopeAngle += s.slopeAngle;
    }

    public void Update(Collision2D c)
    {
        foreach (ContactPoint2D contact in c.contacts)
        {
            Below = Utilities.EqualVectors(Vector2.up, contact.normal);
            Above = Utilities.EqualVectors(Vector2.down, contact.normal);
            Left = Utilities.EqualVectors(Vector2.right, contact.normal);
            Right = Utilities.EqualVectors(Vector2.left, contact.normal);
        }
    }

    public void Reset()
    {
        direction.Clear();
		becameGroundedThisFrame = movingDownSlope = false;
        slopeAngle = 0f;
    }


    public override string ToString()
    {
		var rightStr = FlagsHelper.IsSet(direction.Flags, Direction2D.RIGHT);
		var leftStr = FlagsHelper.IsSet(direction.Flags, Direction2D.LEFT);
		var upStr = FlagsHelper.IsSet(direction.Flags, Direction2D.UP);
		var downStr = FlagsHelper.IsSet(direction.Flags, Direction2D.DOWN);

        return string.Format("[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, becameGroundedThisFrame: {6}",
		                     rightStr, leftStr, upStr, downStr, movingDownSlope, slopeAngle, becameGroundedThisFrame);
    }

	private void SetDirectionForBool(Direction2D flag, bool value)
	{
		if (value)
		{
            direction.Add(flag);
		}
		else
		{
            direction.Subtract(flag);
		}
	}
}
