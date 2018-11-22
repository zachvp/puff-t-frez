using UnityEngine;

/// <summary>
/// Responsible for automatically creating all the core dependencies that need to be present in the scene. This should
/// Be configured to run before any other scripts.
/// </summary>
public class BehaviorInitializer : MonoBehaviour
{
	public MonoBehaviour[] behaviors;

	public void Awake()
	{
		// Spawn all behaviors
		foreach (MonoBehaviour behavior in behaviors)
		{
			Instantiate(behavior, transform);
		}
	}
}
