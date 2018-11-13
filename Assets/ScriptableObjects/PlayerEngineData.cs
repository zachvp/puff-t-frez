using UnityEngine;

public class PlayerEngineData : ScriptableObject
{
	// TODO: Doc these values
	public int collisionBufferMax = 4;

	[Range(2, 20)]
    public int totalHorizontalRays = 8;
    [Range(2, 20)]
    public int totalVerticalRays = 8;

    [Range(0.8f, 0.999f)]
    public float triggerHelperBoxColliderScale = 0.95f;

    /// <summary>
    /// mask with all layers that the player should interact with
    /// </summary>
	public LayerMask platformMask = Constants.Layers.OBSTACLE;

	/// <summary>
    /// mask with all layers that trigger events should fire when intersected
    /// </summary>
    public LayerMask triggerMask = 0;

	/// <summary>
    /// mask with all layers that should act as one-way platforms. Note that one-way platforms should always be EdgeCollider2Ds. This is private because it does not support being
    /// updated anytime outside of the inspector for now.
    /// </summary>
    public LayerMask oneWayPlatformMask = 0;

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

    /// <summary>
    /// the threshold in the change in vertical movement between frames that constitutes jumping
    /// </summary>
    /// <value>The jumping threshold.</value>
    public float jumpingThreshold = 0.07f;


	/// <summary>
    /// the max slope angle that the CC2D can climb
    /// </summary>
    /// <value>The slope limit.</value>
    [Range(0, 90f)]
    public float slopeLimit = 30f;

	/// <summary>
    /// this is used to calculate the downward ray that is cast to check for slopes. We use the somewhat arbitrary value 75 degrees
    /// to calculate the length of the ray that checks for slopes.
    /// </summary>
    public float slopeLimitTangent = Mathf.Tan(75f * Mathf.Deg2Rad);

    /// <summary>
    /// curve for multiplying speed based on slope (negative = down slope and positive = up slope)
    /// </summary>
    public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1.5f), new Keyframe(0, 1), new Keyframe(90, 0));


}
