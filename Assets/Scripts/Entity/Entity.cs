using UnityEngine;

// For any given GameObject in a scene, this handles Transform info.
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

    // Lifecycle events
    // todo: add handling for scene change to null this out (may get complicated with async/concurrent scene loading)
    public static EventHandler<Entity> OnCreate;

    public EventHandler<Vector3> OnScaleChange;

    // Activation events
    public EventHandler<bool> OnActivationChange;
    
	protected CoreTransform oldTransform;

	// Monobehaviour events
    public virtual void Awake()
    {
        oldTransform = new CoreTransform();

        id = idCount;
        idCount++;

        Events.Raise(OnCreate, this);
    }
    
	// ITransform begin
	public virtual void SetPosition(Vector3 p)
    {
		oldTransform.position = transform.position;
        transform.position = p;
    }

    // todo: make virtual
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
