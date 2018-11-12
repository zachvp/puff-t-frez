using UnityEngine;

// For any given GameObject in a scene, this handles position setting, collision
// events, etc.
public class Entity : MonoBehaviour, ITransform, IBehavior
{
	public static long idCount;

	public long id { get; private set; }

	public Affinity affinity { get; private set; }

	public Vector3 Position
	{
    	get { return transform.position; }
		private set { SetPosition(value); }
	}
	public Vector3 LocalScale
	{
		get { return transform.localScale; }
		private set { SetLocalScale(value); }
	}

	public Quaternion Rotation
	{
		get { return transform.rotation; }
		private set { SetRotation(value); }
	}

	public CoreTransform PriorTransform
	{
		get { return oldTransform; }
	}

	public Collider2D Collider
    {
        get { return collider; }
    }

	public int Layer
	{
		get { return gameObject.layer; }
	}

    // Lifecycle events
    // todo: add handling for scene change to null this out (may get complicated with async/concurrent scene loading)
    public static EventHandler<Entity> OnCreate;

    // TODO: remove
    // Trigger events
    public EventHandler<CollisionContextSnapshot> OnTriggerEnter;
	public EventHandler<CollisionContextSnapshot> OnTriggerStay;
	public EventHandler<CollisionContextSnapshot> OnTriggerExit;

	public EventHandler<Vector3> OnScaleChange;

    // Collision events
	// TODO: remove
	public EventHandler<CollisionContextSnapshot> OnCollisionEnter;
	public EventHandler<CollisionContextSnapshot> OnCollisionExit;

    // Activation events
	public EventHandler<bool> OnActivationChange;
    
	new protected Collider2D collider;
	protected CoreTransform oldTransform;

	public CollisionContextSnapshot context { get; private set; }

	// Monobehaviour events
    public void Awake()
    {
        oldTransform = new CoreTransform();
		context = new CollisionContextSnapshot();
        collider = GetComponent<Collider2D>();

        id = idCount;
        idCount++;

        FrameCounter.Instance.OnLateUpdate += HandleLateUpdate;

        Events.Raise(OnCreate, this);
    }

    public void HandleLateUpdate()
    {
		context.Store();
        SetPosition(CoreUtilities.NormalizePosition(Position));
    }

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
    
	// ITransform begin
	public void SetPosition(Vector3 p)
    {
		oldTransform.position = transform.position;
        transform.position = p;
    }

	public void SetLocalScale(Vector3 s)
	{
		oldTransform.localScale = transform.localScale;
		transform.localScale = s;

		Events.Raise(OnScaleChange, s);
	}

	public void SetRotation(Quaternion r)
	{
		oldTransform.rotation = r;
		transform.rotation = r;
	}

    public void SetTransform(CoreTransform t)
    {
        SetPosition(t.position);
        SetRotation(t.rotation);
        SetLocalScale(t.localScale);
    }

    public void SetParent(Transform p)
    {
        oldTransform.parent = p;
        transform.parent = p;
    }

    // ITransform end

	// IBehavior begin - TODO: Remove this interface
    public bool IsActive()
	{
		return isActiveAndEnabled;
	}

	public void SetActive(bool isActive)
	{
		if (gameObject.activeInHierarchy != isActive)
		{
			Events.Raise(OnActivationChange, isActive);

			if (!isActive)
			{
				context.current.Clear();
				context.previous.Clear();
			}
		}

		gameObject.SetActive(isActive);
	}

	public void SetAffinity(Affinity a)
    {
        affinity = a;
    }
	// IBehavior end
    
	// Overrides
	public override bool Equals(object other)
	{
		var cast = other as Entity;
		var result = false;

		if (cast)
		{
			result = id == cast.id;
		}

		return result;
	}

	public override int GetHashCode()
	{
		return id.GetHashCode();
	}
}
