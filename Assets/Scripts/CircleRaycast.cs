using UnityEngine;

public class CircleRaycast : MonoBehaviour
{
	new public CircleCollider2D collider;

	private int theta;

	public void Update()
	{
		var step = 5;
		var radius = transform.localScale.x * collider.radius;
		var distance = 60000;
		var layer = 1 << 8;
		var origin = new Vector2(transform.position.x, transform.position.y);
		var radians = theta * Mathf.Deg2Rad;
		var offset = new Vector2(Mathf.Sin(radians), Mathf.Cos(radians)) * radius;

		var hit = Physics2D.Raycast(origin + offset, Vector2.right, distance, layer);
		Debug.DrawRay(origin + offset, Vector3.right * distance, Color.red);

		theta += step;

		if (theta > 44)
		{
			theta = 0;
		}
	}
}
