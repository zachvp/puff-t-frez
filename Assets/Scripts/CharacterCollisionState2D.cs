public class CharacterCollisionState2D
{
	// TODO: These direction bools can be condensed into flag
    public bool right;
    public bool left;
    public bool above;
    public bool below;
    public bool becameGroundedThisFrame;
    public bool movingDownSlope;
    public float slopeAngle;

    public CharacterCollisionState2D() { }

    public CharacterCollisionState2D(CharacterCollisionState2D other)
    {
        right = other.right;
        left = other.left;
        above = other.above;
        below = other.below;
        becameGroundedThisFrame = other.becameGroundedThisFrame;
        movingDownSlope = other.movingDownSlope;
        slopeAngle = other.slopeAngle;
    }

    public bool hasCollision()
    {
        return below || right || left || above;
    }


    public void reset()
    {
        right = left = above = below = becameGroundedThisFrame = movingDownSlope = false;
        slopeAngle = 0f;
    }


    public override string ToString()
    {
        return string.Format("[CharacterCollisionState2D] r: {0}, l: {1}, a: {2}, b: {3}, movingDownSlope: {4}, angle: {5}, becameGroundedThisFrame: {6}",
                             right, left, above, below, movingDownSlope, slopeAngle, becameGroundedThisFrame);
    }

    public override bool Equals(object obj) {
        var result = false;

        if (obj is Direction2D) {
            var cast = (Direction2D) obj;
            result |= cast == Direction2D.RIGHT && this.right;
            result |= cast == Direction2D.LEFT && this.left;
            result |= cast == Direction2D.ABOVE && this.above;
            result |= cast == Direction2D.BELOW && this.below;
        } else {
            result = base.Equals(obj);
        }

        return result;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
