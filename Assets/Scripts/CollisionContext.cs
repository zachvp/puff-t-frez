using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CollisionContext
{
	public int Count { get { return affinities.Count; } }

	private HashSet<Entity> entities;
	private List<Affinity> affinities;
    private HashSet<LayerMask> layers;
	private HashSet<Collider2D> colliders;
    
    public CollisionContext()
    {
        entities = new HashSet<Entity>();
        affinities = new List<Affinity>();
        layers = new HashSet<LayerMask>();
		colliders = new HashSet<Collider2D>();
    }

	public CollisionContext(CollisionContext other)
	{
		entities = new HashSet<Entity>(other.entities);
		affinities = new List<Affinity>(other.affinities);
		layers = new HashSet<LayerMask>(other.layers);
		colliders = new HashSet<Collider2D>(other.colliders);
	}

	public void Add(Collision2D c)
	{
		Add(c.gameObject.GetComponent<Collider2D>());
	}

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
		Remove(c.gameObject.GetComponent<Collider2D>());
	}

    public void Clear()
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

    public override string ToString()
    {
        return CoreDebug.CollectionString(colliders);
    }

    private void Add(PhysicsEntity e)
    {
        entities.Add(e);
        Add(e.Layer);
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

public class CollisionContextSnapshot
{
	public CollisionContext previous { get; private set; }
	public CollisionContext current  { get; private set; }

	public CollisionContextSnapshot()
	{
		previous = new CollisionContext();
		current = new CollisionContext();
	}

    public void Store()
	{
		previous = new CollisionContext(current);
	}

    public void Clear()
    {
        previous.Clear();
        current.Clear();
    }
}
