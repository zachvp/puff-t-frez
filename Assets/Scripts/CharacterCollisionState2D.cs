using UnityEngine;

public class CharacterCollisionState2D
{
	public Direction2D direction;
	public bool Left
	{
		get { return FlagsHelper.IsSet(direction, Direction2D.LEFT); }
		set { SetDirectionForBool(Direction2D.LEFT, value); }
	}
	public bool Right
	{
		get { return FlagsHelper.IsSet(direction, Direction2D.RIGHT); }
		set { SetDirectionForBool(Direction2D.RIGHT, value); }
	}
	public bool Above
	{
		get { return FlagsHelper.IsSet(direction, Direction2D.UP); }
		set { SetDirectionForBool(Direction2D.UP, value); }
	}
	public bool Below
	{
		get { return FlagsHelper.IsSet(direction, Direction2D.DOWN); }
		set { SetDirectionForBool(Direction2D.DOWN, value); }
	}

    public bool becameGroundedThisFrame;
    public bool movingDownSlope;
    public float slopeAngle;

    public CharacterCollisionState2D() { }

    public CharacterCollisionState2D(CharacterCollisionState2D other)
    {
		direction = other.direction;
        becameGroundedThisFrame = other.becameGroundedThisFrame;
        movingDownSlope = other.movingDownSlope;
        slopeAngle = other.slopeAngle;
    }

	public bool HasCollision()
    {
		return FlagsHelper.IsSet(direction, Direction2D.ALL);
    }


    public void Reset()
    {
		direction = Direction2D.NONE;
		becameGroundedThisFrame = movingDownSlope = false;
        slopeAngle = 0f;
    }


    public override string ToString()
    {
		var rightStr = FlagsHelper.IsSet(direction, Direction2D.RIGHT);
		var leftStr = FlagsHelper.IsSet(direction, Direction2D.LEFT);
		var upStr = FlagsHelper.IsSet(direction, Direction2D.UP);
		var downStr = FlagsHelper.IsSet(direction, Direction2D.DOWN);

        return string.Format("[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, becameGroundedThisFrame: {6}",
		                     rightStr, leftStr, upStr, downStr, movingDownSlope, slopeAngle, becameGroundedThisFrame);
    }

	private void SetDirectionForBool(Direction2D flag, bool value)
	{
		if (value)
		{
			FlagsHelper.Set(ref direction, flag);
		}
		else
		{
			FlagsHelper.Unset(ref direction, flag);
		}
	}
}
