﻿using UnityEngine;
using System;
using System.Collections.Generic;

public class PhysicsEntity : Entity
{
    public PhysicsContextSnapshot<CollisionContext> collision { get; private set; }
    public PhysicsContextSnapshot<PhysicsContext> trigger { get; private set; }

    public int Layer
    {
        get { return 1 << gameObject.layer; }
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

    [NonSerialized]
    new public Collider2D collider;

    private Rigidbody2D body;

    // todo: could probably separate this into its own class
    private LinkedList<CollisionState2D> collisionBuffer;

	public Queue<Callback> tasks;

    // Monobehaviour methods
    public override void Awake()
    {
        base.Awake();

        collision = new PhysicsContextSnapshot<CollisionContext>();
        trigger = new PhysicsContextSnapshot<PhysicsContext>();
        collisionBuffer = new LinkedList<CollisionState2D>();
		tasks = new Queue<Callback>();

		collider = GetComponent<Collider2D>();
        body = GetComponent<Rigidbody2D>();

        FrameCounter.Instance.OnLateUpdate += HandleLateUpdate;
		FrameCounter.Instance.OnFixedUpdate += HandleFixedUpdate;

        OnActivationChange += HandleActivationChange;
    }

    public void HandleFixedUpdate(float deltaTime)
	{        
		foreach (Callback c in tasks)
		{
			c.Fire();
		}

		tasks.Clear();
	}

    // public methods
    public void AddVelocity(Vector2 v)
    {
        SetVelocity(velocity + v);
    }

    public void AddVelocity(float x, float y)
    {
        var v = new Vector2(x, y);

        AddVelocity(v);
    }

    public void SetVelocity(Vector2 v)
    {
		velocity = v;
    }

    public void SetVelocity(float x, float y)
    {
        var v = new Vector2(x, y);

        SetVelocity(v);
    }

    public void ApplyForce(float x, float y)
    {
        var v = new Vector2(x, y);

        body.AddForce(v);
    }

    public override void SetPosition(Vector3 p)
    {
		// Update base transform so the entity at least visually moves to the
        // new location. Body might be a frame or two after.
        base.SetPosition(p);

        body.position = p;
    }

    // todo: should return leftmost collider of four colliders
    public RaycastHit2D Check(CoreDirection direction, float distance)
    {
        ContactFilter2D filter = new ContactFilter2D();
        var results = new RaycastHit2D[1];

        // todo: this should be a param
        filter.layerMask = LayerMask.NameToLayer("Obstacle");

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
            result |= FlagsHelper.IsSet(collisionState.direction.Flags, direction);

            if (result)
            {
                break;
            }
        }

        return result;
    }

    public CollisionState2D GetBufferedCollisionState()
    {
        var result = new CollisionState2D();

        foreach (CollisionState2D collisionState in collisionBuffer)
        {
            result.Add(collisionState);
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
        trigger.current.Add(c);
    }

    public void OnTriggerExit2D(Collider2D c)
    {
        trigger.current.Remove(c);
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
