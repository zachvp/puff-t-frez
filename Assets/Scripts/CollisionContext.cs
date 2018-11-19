using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CollisionContext : PhysicsContext
{
    public CollisionState2D state { get; private set; }

    public CollisionContext()
        : base()
    {
        state = new CollisionState2D();
    }

    public CollisionContext(CollisionContext other)
        : base(other)
    {
        state = new CollisionState2D(other.state);
    }

    public override void Clear()
    {
        base.Clear();

        state.Reset();
    }

    public void Update(CollisionContext c)
    {
        base.Update(c);

        Update(c.state);
    }

    public void Update(CollisionState2D s)
    {
        state.Update(s);
    }
}

public class PhysicsContext
{
	private HashSet<Entity> entities;
	private List<Affinity> affinities;
    private HashSet<LayerMask> layers;
	private HashSet<Collider2D> colliders;

    public PhysicsContext()
    {
        entities = new HashSet<Entity>();
        affinities = new List<Affinity>();
        layers = new HashSet<LayerMask>();
		colliders = new HashSet<Collider2D>();
    }

	public PhysicsContext(PhysicsContext other)
	{
		entities = new HashSet<Entity>(other.entities);
		affinities = new List<Affinity>(other.affinities);
		layers = new HashSet<LayerMask>(other.layers);
		colliders = new HashSet<Collider2D>(other.colliders);
	}

	public void Add(Collision2D c)
	{
        Add(c.collider);
	}

    // todo: shouldn't have all this implicit stuff smh
	public void Add(Collider2D c)
	{
		if (!colliders.Contains(c))
		{
			var checkEntity = c.GetComponent<PhysicsEntity>();

			if (checkEntity)
			{
				Add(checkEntity);
			}
			else
			{
				Add(Affinity.NONE);
				Add(c.gameObject.layer);
			}

			colliders.Add(c);
		}
	}

	public void Remove(Collider2D c)
	{
		if (colliders.Contains(c))
		{
			var checkEntity = c.GetComponent<PhysicsEntity>();

			if (checkEntity)
			{
				Remove(checkEntity);
			}
			else
			{
				Remove(Affinity.NONE);
				Remove(c.gameObject.layer);
			}

			colliders.Remove(c);
		}
	}

	public void Remove(Collision2D c)
	{
		Remove(c.collider);
	}

    public virtual void Clear()
    {
        affinities.Clear();
        entities.Clear();
        layers.Clear();
        colliders.Clear();
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

	public int GetCount(Affinity a)
	{
		return affinities.Count(element => element == a);
	}

    public virtual void Update(PhysicsContext c)
    {
        Update(c.entities);
        Update(c.affinities);
        Update(c.layers);
        Update(c.colliders);
    }

    public void Update(HashSet<Entity> set)
    {
        entities.Clear();
        
        foreach (Entity e in set)
        {
            Add(e);
        }
    }

    public void Update(List<Affinity> set)
    {
        affinities.Clear();

        foreach (Affinity a in set)
        {
            Add(a);
        }
    }

    public void Update(HashSet<LayerMask> set)
    {
        layers.Clear();

        foreach (LayerMask l in set)
        {
            Add(l);
        }
    }

    public void Update(HashSet<Collider2D> set)
    {
        colliders.Clear();

        foreach (Collider2D c in set)
        {
            Add(c);
        }
    }

    public override string ToString()
    {
        return CoreDebug.CollectionString(colliders);
    }

    private void Add(Entity e)
    {
        var checkEntity = e as PhysicsEntity;

        if (checkEntity)
        {
            Add(checkEntity.Layer);
        }

        entities.Add(e);
        Add(e.affinity);
    }

    private void Add(int l)
    {
        layers.Add(l);
    }

    private void Add(Affinity a)
    {
        affinities.Add(a);
    }

    private void Remove(PhysicsEntity e)
    {
        entities.Remove(e);
        Remove(e.Layer);
        Remove(e.affinity);
    }

    private void Remove(int l)
    {
        layers.Remove(l);
    }

    private void Remove(Affinity a)
    {
        affinities.Remove(a);
    }
}

// todo: should be templated PhysicsContextSnapshot
public class PhysicsContextSnapshot<T> where T : PhysicsContext, new()
{
	public T previous { get; private set; }
	public T current  { get; private set; }

	public PhysicsContextSnapshot()
	{
		previous = new T();
		current = new T();
	}

    public void Store()
	{
		previous.Update(current);
	}

    public void Clear()
    {
        previous.Clear();
        current.Clear();
    }
}
