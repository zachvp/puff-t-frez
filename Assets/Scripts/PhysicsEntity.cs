using UnityEngine;

public class PhysicsEntity : Entity
{
    public CollisionContextSnapshot context { get; private set; }

    private Rigidbody2D body;

    public int Layer
    {
        get { return gameObject.layer; }
    }

    // Monobehaviour methods
    public override void Awake()
    {
        base.Awake();

        context = new CollisionContextSnapshot();

        body = GetComponent<Rigidbody2D>();

        FrameCounter.Instance.OnLateUpdate += HandleLateUpdate;
        FrameCounter.Instance.OnUpdate += HandleUpdate;

        OnActivationChange += HandleActivationChange;
    }

    // Public methods
    public void SetVelocity(Vector2 v)
    {
        body.velocity = v;
    }

    // Frame events
    public void HandleUpdate(long currentFrame, float deltaTime)
    {
        // Update collision state
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
    public void OnTriggerEnter2D(Collider2D c)
    {
        context.current.Add(c);
    }

    public void OnTriggerExit2D(Collider2D c)
    {
        context.current.Remove(c);
    }

    public void OnCollisionEnter2D(Collision2D c)
    {
        // Update collision context;
        context.current.Add(c);
        context.current.state.Update(c);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        context.current.Remove(collision);
        context.current.state.Reset();
    }
}
