using UnityEngine;

public class Test : MonoBehaviour
{
	public int speed = 300;
	public Vector3 dir = Vector3.right;

	CollisionContext context;

	private Vector3 originalPosition;

	public void Awake()
	{
		context = new CollisionContext();
		originalPosition = transform.position;
	}

	public void Update()
    {
		if (Input.GetKeyDown(KeyCode.Return))
		{
			transform.position = originalPosition;
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			speed += 50;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			speed -= 50;
		}

		var newPos = transform.position;

		newPos += dir * speed * Time.deltaTime;

		transform.position = CoreUtilities.NormalizePosition(newPos);
    }

	public void FixedUpdate()
	{
		
	}

	public void OnTriggerEnter2D(Collider2D collider)
    {
        context.Add(collider);
    }

    public void OnTriggerStay2D(Collider2D collider)
    {
        context.Add(collider);
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        context.Remove(collider);
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        context.Add(collision);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        context.Remove(collision);
    }

	private void RaycastTest()
	{
        RaycastHit2D hit;
        var direction = transform.TransformDirection(Vector3.down);
        var distance = 1024;

        // Does the ray intersect any objects excluding the player layer
		// Bit shift the index of the layer (8) to get a bit mask
        if (hit = Physics2D.Raycast(transform.position, direction, distance, 1 << 8))
        {
            var layer = hit.collider.gameObject.layer;
            var layerName = LayerMask.LayerToName(layer);

            Debug.DrawRay(transform.position, direction * hit.distance, Color.yellow);
            Debug.LogFormat("Did Hit. Layer: {0} name: {1}", layer, layerName);
        }
        else
        {
            Debug.DrawRay(transform.position, direction * 1000, Color.white);
            Debug.Log("Did not Hit");
        }
	}
}