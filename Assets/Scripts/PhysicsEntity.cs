using UnityEngine;

public class PhysicsEntity : Entity
{
    // Trigger collider events
    public EventHandler<CollisionContextSnapshot> OnTriggerEnter;
    public EventHandler<CollisionContextSnapshot> OnTriggerStay;
    public EventHandler<CollisionContextSnapshot> OnTriggerExit;

    // Collision collider events
    public EventHandler<CollisionContextSnapshot> OnCollisionEnter;
    public EventHandler<CollisionContextSnapshot> OnCollisionExit;

    public CollisionContextSnapshot context { get; private set; }

    private Rigidbody2D body;

    public int Layer
    {
        get { return gameObject.layer; }
    }

    public override void Awake()
    {
        base.Awake();

        context = new CollisionContextSnapshot();

        FrameCounter.Instance.OnLateUpdate += HandleLateUpdate;
        OnActivationChange += HandleActivationChange;
    }

    public void HandleLateUpdate()
    {
        context.Store();
    }

    // Entity events
    public void HandleActivationChange(bool isActive)
    {
        if (!isActive)
        {
            context.current.Clear();
            context.previous.Clear();
        }
    }

    // Collider events
    public void OnTriggerEnter2D(Collider2D collider)
    {
        context.current.Add(collider);
        Events.Raise(OnTriggerEnter, context);
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        context.current.Add(collider);
        Events.Raise(OnTriggerStay, context);
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        context.current.Remove(collider);
        Events.Raise(OnTriggerExit, context);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.LogWarning("did you mean to call OnTriggerEnter instead?");

        context.current.Add(collision);
        Events.Raise(OnCollisionExit, context);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        Debug.LogWarning("did you mean to call OnTriggerExit instead?");

        context.current.Remove(collision);
        Events.Raise(OnCollisionExit, context);
    }
}
