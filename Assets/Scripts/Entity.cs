using UnityEngine;

// For any given GameObject in a scene, this handles position setting, collision
// events, etc.
public class Entity : MonoBehaviour, ITransform, IBehavior {
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

	public Collider2D Collider
	{
		get { return _collider; }
	}

	public StoreTransform PriorTransform
	{
		get { return _priorTransform; }
	}

	// Trigger events
	public EventHandler<Collider2D> OnTriggerEnter;
	public EventHandler<Collider2D> OnTriggerStay;
	public EventHandler<Collider2D> OnTriggerExit;

	public EventHandler<Vector3> OnScaleChange;

    // Collision events
	public EventHandler<Collision2D> OnCollisionEnter;

	protected Collider2D _collider;
	protected StoreTransform _priorTransform;

	// MonoBehaviour begin
	public void LateUpdate()
	{
		SetPosition(CoreUtilities.NormalizePosition(Position));
	}
	// MonoBehaviour end

	// ITransform begin
	public void SetPosition(Vector3 position)
    {
		_priorTransform.position = transform.position;
        transform.position = position;
    }

	public void SetLocalScale(Vector3 scale)
	{
		_priorTransform.localScale = transform.localScale;
		transform.localScale = scale;

		Events.Raise(OnScaleChange, scale);
	}

	public void SetRotation(Quaternion rotation)
	{
		_priorTransform.rotation = rotation;
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
	public void Awake()
	{
		_priorTransform = new StoreTransform();

		_collider = GetComponent<Collider2D>();
	}

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
