using UnityEngine;
using System.Collections.Generic;

public class PhysicsEntity : Entity
{
    public CollisionContextSnapshot collision { get; private set; }

    public int Layer
    {
        get { return gameObject.layer; }
    }

    public Vector2 velocity
    {
        get
        {
            return body.velocity;
        }
        private set
        {
            body.velocity = value;
        }
    }

    private Rigidbody2D body;

    private LinkedList<CollisionState2D> collisionBuffer;

    // Monobehaviour methods
    public override void Awake()
    {
        base.Awake();

        collision = new CollisionContextSnapshot();
        collisionBuffer = new LinkedList<CollisionState2D>();

        body = GetComponent<Rigidbody2D>();

        FrameCounter.Instance.OnLateUpdate += HandleLateUpdate;

        OnActivationChange += HandleActivationChange;
    }

    // public methods
    public void AddVelocity(float x, float y)
    {
        var v = body.velocity;

        v.x += x;
        v.y += y;

        body.velocity = v;
    }

    public void SetVelocity(Vector2 v)
    {
        velocity = v;
    }

    public void SetVelocity(float x, float y)
    {
        var v = body.velocity;

        v.x = x;
        v.y = y;

        body.velocity = v;
    }

    // todo: should return leftmost collider of four colliders
    public RaycastHit2D Check(CoreDirection direction, float distance)
    {
        ContactFilter2D filter = new ContactFilter2D();
        var results = new RaycastHit2D[1];

        filter.layerMask = Constants.Layers.OBSTACLE;

        body.Cast(direction.Vector, filter, results, distance);

        // Return the first index
        return results[0];
    }

    public CollisionState2D CheckProximity(float distance, Direction2D mask)
    {
        var proximityCollision = new CollisionState2D();

        // Check below
        if (FlagsHelper.IsSet(mask, Direction2D.DOWN))
        {
            proximityCollision.Below = Check(Constants.Directions.DOWN, distance);
        }

        // Check above
        if (FlagsHelper.IsSet(mask, Direction2D.UP))
        {
            proximityCollision.Above = Check(Constants.Directions.UP, distance);
        }

        // Check right.
        if (FlagsHelper.IsSet(mask, Direction2D.RIGHT))
        {
            proximityCollision.Right = Check(Constants.Directions.RIGHT, distance);
        }

        // Check left
        if (FlagsHelper.IsSet(mask, Direction2D.LEFT))
        {
            proximityCollision.Left = Check(Constants.Directions.LEFT, distance);
        }

        return proximityCollision;
    }

    public bool IsCollisionBuffered(Direction2D direction)
    {
        var result = false;

        foreach (CollisionState2D collisionState in collisionBuffer)
        {
            result |= FlagsHelper.IsSet(collisionState.direction, direction);
        }

        return result;
    }

    public void HandleLateUpdate()
    {
        // Update current collision state.
        collision.current.state.Update(CheckProximity(8, Direction2D.ALL));

        // Add to the collision buffer.
        // todo: should just update pre-existing states in the buffer so not allocating new instances every frame
        collisionBuffer.AddFirst(new CollisionState2D(collision.current.state));

        if (collisionBuffer.Count > 4)
        {
            collisionBuffer.RemoveLast();
        }

        collision.Store();
    }

    // Entity events
    public void HandleActivationChange(bool isActive)
    {
        if (!isActive)
        {
            collision.current.Clear();
            collision.previous.Clear();
        }
    }

    // Collider events
    public void OnTriggerEnter2D(Collider2D c)
    {
        collision.current.Add(c);
    }

    public void OnTriggerExit2D(Collider2D c)
    {
        collision.current.Remove(c);
    }

    public void OnCollisionEnter2D(Collision2D c)
    {
        collision.current.Add(c);
    }

    public void OnCollisionExit2D(Collision2D c)
    {
        this.collision.current.Remove(c);
    }
}
