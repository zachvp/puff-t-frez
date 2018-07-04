using UnityEngine;

// For any given GameObject in a scene, this handles position setting, collision
// events, etc.
public class Entity : MonoBehaviour, ITransform {
	public Vector3 position {
    	get { return transform.position; }
		private set { transform.position = value; }
	}
	public Vector3 localScale { 
		get { return transform.localScale; }
		private set { transform.localScale = value; }
	}

	// Trigger events
	public EventHandler<Collider2D> OnTriggerEnter;
	public EventHandler<Collider2D> OnTriggerStay;
	public EventHandler<Collider2D> OnTriggerExit;

    // Collision events
	public EventHandler<Collision2D> OnCollisionEnter;

	// ITransform functions
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

	public void SetLocalScale(Vector3 scale) {
		transform.localScale = scale;
	}

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
