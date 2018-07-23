using UnityEngine;
using System.Collections.Generic;

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

	public StoreTransform PriorTransform
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
        
	// Trigger events
	public EventHandler<CollisionContext> OnTriggerEnter;
	public EventHandler<CollisionContext> OnTriggerStay;
	public EventHandler<CollisionContext> OnTriggerExit;

	public EventHandler<Vector3> OnScaleChange;

    // Collision events
	public EventHandler<CollisionContext> OnCollisionEnter;

	public EventHandler<bool> OnActivationChange;
    
	new protected Collider2D collider;
	protected StoreTransform oldTransform;

	public void SetAffinity(Affinity affinityPrime)
	{
		affinity = affinityPrime;
	}
    
	// ITransform begin
	public void SetPosition(Vector3 position)
    {
		oldTransform.position = transform.position;
        transform.position = position;
    }

	public void SetLocalScale(Vector3 scale)
	{
		oldTransform.localScale = transform.localScale;
		transform.localScale = scale;

		Events.Raise(OnScaleChange, scale);
	}

	public void SetRotation(Quaternion rotation)
	{
		oldTransform.rotation = rotation;
		transform.rotation = rotation;
	}
    // ITransform end

    // IBehavior begin
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
	// IBehavior end

	// Monobehaviour events
	public void Awake()
	{
		oldTransform = new StoreTransform();
		collider = GetComponent<Collider2D>();

		id = idCount;
        idCount++;
	}

	public void LateUpdate()
    {
        SetPosition(CoreUtilities.NormalizePosition(Position));
    }

	public void OnTriggerEnter2D(Collider2D collider)
	{
		Events.Raise(OnTriggerEnter, new CollisionContext(collider));
	}

	public void OnTriggerStay2D(Collider2D collider)
	{
		Events.Raise(OnTriggerStay, new CollisionContext(collider));
	}

	public void OnTriggerExit2D(Collider2D collider)
	{
		Events.Raise(OnTriggerExit, new CollisionContext(collider));

	}

	public void OnCollisionEnter2D(Collision2D collision)
    {
		Debug.LogWarning("did you mean to call OnTriggerEnter instead?");
		Events.Raise(OnTriggerExit, new CollisionContext(collision));
    }
        
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

public enum Affinity
{
	NONE = 0,
	PLAYER = 1,
    NPC = 2
}

public class CollisionContext
{
	private HashSet<Entity> entities;
	private HashSet<LayerMask> layers;
	private List<Affinity> affinities;

	public CollisionContext()
	{
		entities = new HashSet<Entity>();
		affinities = new List<Affinity>();
		layers = new HashSet<LayerMask>();
	}

	public CollisionContext(Collider2D c) : this()
	{
		var checkEntity = c.GetComponent<Entity>();
        
        if (checkEntity)
        {
			Add(checkEntity);
        }
        else
        {
			Add(Affinity.NONE);
			Add(c.gameObject.layer);
        }
	}

	public CollisionContext(Collision2D c)
		: this(c.gameObject.GetComponent<Collider2D>())
	{ }

	public void Add(Entity e)
	{
		entities.Add(e);
		Add(e.Layer);
		Add(e.affinity);
	}

	public void Add(int l)
	{
		layers.Add(l);
	}

	public void Add(Affinity a)
	{
		affinities.Add(a);
	}

	public void Remove(Entity e)
    {
		entities.Remove(e);
		Remove(e.Layer);
		Remove(e.affinity);
    }

	public void Remove(int l)
    {
		layers.Remove(l);
    }

	public void Remove(Affinity a)
	{
		affinities.Remove(a);
	}

    public void Clear()
	{
		affinities.Clear();
		entities.Clear();
		layers.Clear();
	}

	public bool IsColliding(Affinity a)
    {
		return affinities.Contains(a);
    }

	public bool IsColliding(LayerMask layer)
	{
		return layers.Contains(layer);
	}

	public bool IsColliding(Entity e)
	{
		return entities.Contains(e);
	}
}
