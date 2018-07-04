using UnityEngine;

public class PlayerEngineData : ScriptableObject
{
	// TODO: Doc these values
	public int collisionBufferMax = 4;

	/// <summary>
    /// toggles if the RigidBody2D methods should be used for movement or if Transform.Translate will be used. All the usual Unity rules for physics based movement apply when true
    /// such as getting your input in Update and only calling move in FixedUpdate amonst others.
    /// </summary>
	public bool usePhysicsForMovement = false;

	/// <summary>
    /// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
    /// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
    /// </summary>
    public float skinWidth = 1f;
}
