using UnityEngine;

// For any given GameObject in a scene, this handles position setting, collision
// events, etc.
public class Entity : MonoBehaviour, ITransform, IBehavior {
	public Vector3 position
	{
    	get { return transform.position; }
		private set { SetPosition(value); }
	}
	public Vector3 localScale
	{
		get { return transform.localScale; }
		private set { SetLocalScale(value); }
	}
	public Quaternion rotation
	{
		get { return transform.rotation; }
		private set { SetRotation(value); }
	}

	// Trigger events
	public EventHandler<Collider2D> OnTriggerEnter;
	public EventHandler<Collider2D> OnTriggerStay;
	public EventHandler<Collider2D> OnTriggerExit;

    // Collision events
	public EventHandler<Collision2D> OnCollisionEnter;

	// ITransform begin
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

	public void SetLocalScale(Vector3 scale)
	{
		transform.localScale = scale;
	}

	public void SetRotation(Quaternion rotation)
	{
		transform.rotation = rotation;
	}
    // ITransform end

    // IBehavior begin
	public void SetActive(bool isActive)
	{
		gameObject.SetActive(isActive);
	}
    // IBehavior end

    // Monobehaviour events
	public void OnTriggerEnter2D(Collider2D collider)
	{
		Events.Raise(OnTriggerEnter, collider);
	}

	public void OnTriggerStay2D(Collider2D collision)
	{
		Events.Raise(OnTriggerStay, collision);
	}

	public void OnTriggerExit2D(Collider2D collider)
	{
		Events.Raise(OnTriggerExit, collider);
	}

	public void OnCollisionEnter2D(Collision2D collision)
    {
		Events.Raise(OnCollisionEnter, collision);
    }
}
