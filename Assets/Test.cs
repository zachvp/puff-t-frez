using UnityEngine;

public class Test : MonoBehaviour
{
    // C# example.
    void Update()
    {
        // Bit shift the index of the layer (8) to get a bit mask
		RaycastHit2D hit;
		LayerMask mask = LayerMask.NameToLayer("Obstacle");
		var direction = transform.TransformDirection(Vector3.down);
		var distance = 1024;

		// Does the ray intersect any objects excluding the player layer
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