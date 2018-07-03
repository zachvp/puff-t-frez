#define DEBUG_CC2D_RAYS
using UnityEngine;
using System;
using System.Collections.Generic;

[RequireComponent( typeof( BoxCollider2D ), typeof( Rigidbody2D ) )]
public class CharacterController2D : MonoBehaviour
{
	public const int COLLISION_BUFFER_MAX = 4;

	#region internal types

	private struct CharacterRaycastOrigins
	{
		public Vector3 topLeft;
		public Vector3 bottomRight;
		public Vector3 bottomLeft;
	}

	#endregion


	#region events, properties and fields

	public event Action<RaycastHit2D> onControllerCollidedEvent;
	public event Action<Collider2D> onTriggerEnterEvent;
	public event Action<Collision2D> onCollisionEnterEvent;
	public event Action<Collider2D> onTriggerStayEvent;
	public event Action<Collider2D> onTriggerExitEvent;


	/// <summary>
	/// toggles if the RigidBody2D methods should be used for movement or if Transform.Translate will be used. All the usual Unity rules for physics based movement apply when true
	/// such as getting your input in Update and only calling move in FixedUpdate amonst others.
	/// </summary>
	public bool usePhysicsForMovement = false;

	[SerializeField]
	[Range( 0.001f, 8f )]
	private float _skinWidth = 0.02f;

	/// <summary>
	/// defines how far in from the edges of the collider rays are cast from. If cast with a 0 extent it will often result in ray hits that are
	/// not desired (for example a foot collider casting horizontally from directly on the surface can result in a hit)
	/// </summary>
	public float skinWidth
	{
		get { return _skinWidth; }
		set
		{
			_skinWidth = value;
			recalculateDistanceBetweenRays();
		}
	}


	/// <summary>
	/// mask with all layers that the player should interact with
	/// </summary>
	public LayerMask platformMask = 0;

	/// <summary>
	/// mask with all layers that trigger events should fire when intersected
	/// </summary>
	public LayerMask triggerMask = 0;

	/// <summary>
	/// mask with all layers that should act as one-way platforms. Note that one-way platforms should always be EdgeCollider2Ds. This is private because it does not support being
	/// updated anytime outside of the inspector for now.
	/// </summary>
	[SerializeField]
	private LayerMask oneWayPlatformMask = 0;

	/// <summary>
	/// the max slope angle that the CC2D can climb
	/// </summary>
	/// <value>The slope limit.</value>
	[Range( 0, 90f )]
	public float slopeLimit = 30f;

	/// <summary>
	/// the threshold in the change in vertical movement between frames that constitutes jumping
	/// </summary>
	/// <value>The jumping threshold.</value>
	public float jumpingThreshold = 0.07f;


	/// <summary>
	/// curve for multiplying speed based on slope (negative = down slope and positive = up slope)
	/// </summary>
	public AnimationCurve slopeSpeedMultiplier = new AnimationCurve( new Keyframe( -90, 1.5f ), new Keyframe( 0, 1 ), new Keyframe( 90, 0 ) );

	[Range( 2, 20 )]
	public int totalHorizontalRays = 8;
	[Range( 2, 20 )]
	public int totalVerticalRays = 4;


	/// <summary>
	/// this is used to calculate the downward ray that is cast to check for slopes. We use the somewhat arbitrary value 75 degrees
	/// to calculate the length of the ray that checks for slopes.
	/// </summary>
	private float _slopeLimitTangent = Mathf.Tan( 75f * Mathf.Deg2Rad );

	[Range( 0.8f, 0.999f )]
	public float triggerHelperBoxColliderScale = 0.95f;


	[HideInInspector][NonSerialized]
	public new Transform transform;
	[HideInInspector][NonSerialized]
	public BoxCollider2D boxCollider;
	[HideInInspector][NonSerialized]
	public Rigidbody2D rigidBody2D;

	[HideInInspector][NonSerialized]
	public CharacterCollisionState2D collision = new CharacterCollisionState2D();
	[HideInInspector][NonSerialized]
	public Vector3 velocity;
    public bool isGrounded { get { return collision.below; } }

	#endregion


	/// <summary>
	/// holder for our raycast origin corners (TR, TL, BR, BL)
	/// </summary>
	private CharacterRaycastOrigins _raycastOrigins;

	/// <summary>
	/// stores our raycast hit during movement
	/// </summary>
	private RaycastHit2D _raycastHit;

	/// <summary>
	/// stores any raycast hits that occur this frame. we have to store them in case we get a hit moving
	/// horizontally and vertically so that we can send the events after all collision state is set
	/// </summary>
	private List<RaycastHit2D> _raycastHitsThisFrame = new List<RaycastHit2D>( 2 );

	// horizontal/vertical movement data
	private float _verticalDistanceBetweenRays;
	private float _horizontalDistanceBetweenRays;
	// we use this flag to mark the case where we are travelling up a slope and we modified our delta.y to allow the climb to occur.
	// the reason is so that if we reach the end of the slope we can make an adjustment to stay grounded
	private bool _isGoingUpSlope = false;

	private List<CharacterCollisionState2D> collisionBuffer;

	#region Monobehaviour

	void Awake()
	{
		// add our one-way platforms to our normal platform mask so that we can land on them from above
		platformMask |= oneWayPlatformMask;

		// cache some components
		transform = GetComponent<Transform>();
		boxCollider = GetComponent<BoxCollider2D>();
		rigidBody2D = GetComponent<Rigidbody2D>();

		// here, we trigger our properties that have setters with bodies
		skinWidth = _skinWidth;

		collisionBuffer = new List<CharacterCollisionState2D>(4);
	}

	public bool isCollisionBuffered(Direction2D direction, int windowSize) {
		Debug.AssertFormat(windowSize <= collisionBuffer.Count, "You blew it using collisionBuffer. Requested window exceeds buffer size.");

		var result = false;

		for (var i = collisionBuffer.Count - 1; i > 0; --i) {
			var collision = collisionBuffer[i];

			if (collision.Equals(direction)) {
				Debug.LogFormat("DBG: Collision found in buffer");
				result = true;
				break;
			}
		}

		return result;
	}

	public bool isCollisionBuffered(Direction2D direction) {
		return this.isCollisionBuffered(direction, collisionBuffer.Count);
	}

	public void OnTriggerEnter2D( Collider2D col )
	{
		// Debug.Log("OnTriggerEnterEvent");
		if( onTriggerEnterEvent != null )
			onTriggerEnterEvent( col );
	}

	public void OnCollisionEnter2D( Collision2D collision )
	{
		if( onCollisionEnterEvent != null )
			onCollisionEnterEvent( collision );
	}


	public void OnTriggerStay2D( Collider2D col )
	{
		if( onTriggerStayEvent != null )
			onTriggerStayEvent( col );
	}


	public void OnTriggerExit2D( Collider2D col )
	{
		if( onTriggerExitEvent != null )
			onTriggerExitEvent( col );
	}

	#endregion


	//[System.Diagnostics.Conditional( "DEBUG_CC2D_RAYS" )]
	private void DrawRay( Vector3 start, Vector3 dir, Color color )
	{
		Debug.DrawRay( start, dir, color );
	}


	#region Public

	/// <summary>
	/// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
	/// stop when run into.
	/// </summary>
	/// <param name="deltaMovement">Delta movement.</param>
	public void move( Vector3 deltaMovement)
	{
        // save off our current grounded state which we will use for becameGroundedThisFrame
        var oldCollisionState = new CharacterCollisionState2D(collision);

		// clear our state
		collision.reset();
		_raycastHitsThisFrame.Clear();
		_isGoingUpSlope = false;

		var desiredPosition = transform.position + deltaMovement;
		primeRaycastOrigins( desiredPosition, deltaMovement );

		// first, we check for a slope below us before moving
		// only check slopes if we are going down and grounded
        if( deltaMovement.y < 0 && oldCollisionState.below)
			handleVerticalSlope( ref deltaMovement );

		// now we check movement in the horizontal dir
        if( Mathf.Abs(deltaMovement.x) > 0 )
            moveHorizontally( ref deltaMovement, oldCollisionState );

		// next, check movement in the vertical dir
        if( Mathf.Abs(deltaMovement.y) > 0 )
            moveVertically( ref deltaMovement, oldCollisionState );

		// move then update our state
		if( usePhysicsForMovement )
		{
			GetComponent<Rigidbody2D>().MovePosition( transform.position + deltaMovement );
			velocity = GetComponent<Rigidbody2D>().velocity;
		}
		else
		{
			transform.Translate( deltaMovement, Space.World );
			transform.position = CoreUtilities.NormalizePosition(transform.position);

			// only calculate velocity if we have a non-zero deltaTime
            if( FrameCounter.Instance.deltaTime > 0 ) {
                velocity = deltaMovement / Time.deltaTime;
            }
		}

        // After translation, update proximity check so collision state is fresh.
        // Without this, collision state doesn't get updated if there's no delta movement.
        checkProximity();

		// Add to the collision buffer.
		collisionBuffer.Add(new CharacterCollisionState2D(collision));

		if (collisionBuffer.Count > COLLISION_BUFFER_MAX) {
			// TODO: Change collision buffer to be linked list so this is better
			collisionBuffer.RemoveAt(0);
		}

		// set our becameGrounded state based on the previous and current collision state
        if( !oldCollisionState.below && collision.below )
			collision.becameGroundedThisFrame = true;

		// if we are going up a slope we artificially set a y velocity so we need to zero it out here
		if( _isGoingUpSlope )
			velocity.y = 0;

		// send off the collision events if we have a listener
		if( onControllerCollidedEvent != null )
		{
			for( var i = 0; i < _raycastHitsThisFrame.Count; i++ )
				onControllerCollidedEvent( _raycastHitsThisFrame[i] );
		}
	}


	/// <summary>
	/// moves directly down until grounded
	/// </summary>
	public void warpToGrounded()
	{
		do
		{
            move( new Vector3( 0, -1f, 0 ) );
		} while( !isGrounded );
	}


	/// <summary>
	/// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
	/// It is also used in the skinWidth setter in case it is changed at runtime.
	/// </summary>
	public void recalculateDistanceBetweenRays()
	{
		// figure out the distance between our rays in both directions
		// horizontal
		var colliderUseableHeight = boxCollider.size.y * Mathf.Abs( transform.localScale.y ) - ( 2f * _skinWidth );
		_verticalDistanceBetweenRays = colliderUseableHeight / ( totalHorizontalRays - 1 );

		// vertical
		var colliderUseableWidth = boxCollider.size.x * Mathf.Abs( transform.localScale.x ) - ( 2f * _skinWidth );
		_horizontalDistanceBetweenRays = colliderUseableWidth / ( totalVerticalRays - 1 );
	}

	#endregion


	#region Private Movement Methods

	/// <summary>
	/// resets the raycastOrigins to the current extents of the box collider inset by the skinWidth. It is inset
	/// to avoid casting a ray from a position directly touching another collider which results in wonky normal data.
	/// </summary>
	/// <param name="futurePosition">Future position.</param>
	/// <param name="deltaMovement">Delta movement.</param>
	private void primeRaycastOrigins( Vector3 futurePosition, Vector3 deltaMovement )
	{
		// our raycasts need to be fired from the bounds inset by the skinWidth
		var modifiedBounds = boxCollider.bounds;
		modifiedBounds.Expand( -2f * _skinWidth );

		_raycastOrigins.topLeft = new Vector2( modifiedBounds.min.x, modifiedBounds.max.y );
		_raycastOrigins.bottomRight = new Vector2( modifiedBounds.max.x, modifiedBounds.min.y );
		_raycastOrigins.bottomLeft = modifiedBounds.min;
	}

	/// <summary>
	/// we have to use a bit of trickery in this one. The rays must be cast from a small distance inside of our
	/// collider (skinWidth) to avoid zero distance rays which will get the wrong normal. Because of this small offset
	/// we have to increase the ray distance skinWidth then remember to remove skinWidth from deltaMovement before
	/// actually moving the player
	/// </summary>
    private void moveHorizontally( ref Vector3 deltaMovement, CharacterCollisionState2D oldCollisionState )
	{
		var isGoingRight = deltaMovement.x > 0;
		var rayDistance = Mathf.Abs( deltaMovement.x ) + _skinWidth;
		var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		var initialRayOrigin = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

		for( var i = 0; i < totalHorizontalRays; i++ )
		{
			var ray = new Vector2( initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays );

			//DrawRay( ray, rayDirection * rayDistance, Color.red );

			// if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
			// walk up sloped oneWayPlatforms
            if( i == 0 && oldCollisionState.below) {
				_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, platformMask );
			} else {
				_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, platformMask & ~oneWayPlatformMask);
			}

			if( _raycastHit )
			{
				// the bottom ray can hit slopes but no other ray can so we have special handling for those cases
				if( i == 0 && handleHorizontalSlope( ref deltaMovement, Vector2.Angle( _raycastHit.normal, Vector2.up ) ) )
				{
					_raycastHitsThisFrame.Add( _raycastHit );
					break;
				}

				// set our new deltaMovement and recalculate the rayDistance taking it into account
				deltaMovement.x = _raycastHit.point.x - ray.x;
				rayDistance = Mathf.Abs( deltaMovement.x );

				// remember to remove the skinWidth from our deltaMovement
				if( isGoingRight )
				{
					deltaMovement.x -= _skinWidth;
					collision.right = true;
				}
				else
				{
					deltaMovement.x += _skinWidth;
					collision.left = true;
				}

				_raycastHitsThisFrame.Add( _raycastHit );
			}
		}
	}


	/// <summary>
	/// handles adjusting deltaMovement if we are going up a slope.
	/// </summary>
	/// <returns><c>true</c>, if horizontal slope was handled, <c>false</c> otherwise.</returns>
	/// <param name="deltaMovement">Delta movement.</param>
	/// <param name="angle">Angle.</param>
	private bool handleHorizontalSlope( ref Vector3 deltaMovement, float angle )
	{
		// disregard 90 degree angles (walls)
		if( Mathf.RoundToInt( angle ) == 90 )
			return false;

		// if we can walk on slopes and our angle is small enough we need to move up
		if( angle < slopeLimit )
		{
			// we only need to adjust the deltaMovement if we are not jumping
			// TODO: this uses a magic number which isn't ideal!
			if( deltaMovement.y < jumpingThreshold )
			{
				// apply the slopeModifier to slow our movement up the slope
				var slopeModifier = slopeSpeedMultiplier.Evaluate( angle );
				deltaMovement.x *= slopeModifier;

				// we dont set collisions on the sides for this since a slope is not technically a side collision

				// smooth y movement when we climb. we make the y movement equivalent to the actual y location that corresponds
				// to our new x location using our good friend Pythagoras
				deltaMovement.y = Mathf.Abs( Mathf.Tan( angle * Mathf.Deg2Rad ) * deltaMovement.x );
				_isGoingUpSlope = true;

				collision.below = true;
			}
		}
		else // too steep. get out of here
		{
			deltaMovement.x = 0;
		}

		return true;
	}

    private void moveVertically( ref Vector3 deltaMovement, CharacterCollisionState2D oldCollisionState )
	{
		var isGoingUp = deltaMovement.y > 0;
		var rayDistance = Mathf.Abs( deltaMovement.y ) + _skinWidth;
		var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

		// apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
		initialRayOrigin.x += deltaMovement.x;

		// if we are moving up, we should ignore the layers in oneWayPlatformMask
		var mask = platformMask;
        if( isGoingUp && !oldCollisionState.below)
			mask &= ~oneWayPlatformMask;

		for( var i = 0; i < totalVerticalRays; i++ )
		{
			var ray = new Vector2( initialRayOrigin.x + i * _horizontalDistanceBetweenRays, initialRayOrigin.y );

            DrawRay( ray, rayDirection * rayDistance, Color.blue );
			_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, mask );

			if( _raycastHit )
			{
				// set our new deltaMovement and recalculate the rayDistance taking it into account
				deltaMovement.y = _raycastHit.point.y - ray.y;
				rayDistance = Mathf.Abs( deltaMovement.y );

				// remember to remove the skinWidth from our deltaMovement
				if( isGoingUp )
				{
					deltaMovement.y -= _skinWidth;
					collision.above = true;
				}
				else
				{
					deltaMovement.y += _skinWidth;
					collision.below = true;
				}

				_raycastHitsThisFrame.Add( _raycastHit );

				// this is a hack to deal with the top of slopes. if we walk up a slope and reach the apex we can get in a situation
				// where our ray gets a hit that is less then skinWidth causing us to be ungrounded the next frame due to residual velocity.
				if( !isGoingUp && deltaMovement.y > 0.00001f )
					_isGoingUpSlope = true;
			}
		}
	}


	/// <summary>
	/// checks the center point under the BoxCollider2D for a slope. If it finds one then the deltaMovement is adjusted so that
	/// the player stays grounded and the slopeSpeedModifier is taken into account to speed up movement.
	/// </summary>
	/// <param name="deltaMovement">Delta movement.</param>
	private void handleVerticalSlope( ref Vector3 deltaMovement )
	{
		// slope check from the center of our collider
		var centerOfCollider = ( _raycastOrigins.bottomLeft.x + _raycastOrigins.bottomRight.x ) * 0.5f;
		var rayDirection = -Vector2.up;

		// the ray distance is based on our slopeLimit
		var slopeCheckRayDistance = _slopeLimitTangent * ( _raycastOrigins.bottomRight.x - centerOfCollider );

		var slopeRay = new Vector2( centerOfCollider, _raycastOrigins.bottomLeft.y );
		DrawRay( slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow );
		_raycastHit = Physics2D.Raycast( slopeRay, rayDirection, slopeCheckRayDistance, platformMask );
		if( _raycastHit )
		{
			// bail out if we have no slope
			var angle = Vector2.Angle( _raycastHit.normal, Vector2.up );
			if( angle == 0 )
				return;

			// we are moving down the slope if our normal and movement direction are in the same x direction
			var isMovingDownSlope = Mathf.Sign( _raycastHit.normal.x ) == Mathf.Sign( deltaMovement.x );
			if( isMovingDownSlope )
			{
				// going down we want to speed up in most cases so the slopeSpeedMultiplier curve should be > 1 for negative angles
				var slopeModifier = slopeSpeedMultiplier.Evaluate( -angle );
				deltaMovement.y = _raycastHit.point.y - slopeRay.y - skinWidth;
				deltaMovement.x *= slopeModifier;
				collision.movingDownSlope = true;
				collision.slopeAngle = angle;
			}
		}
	}

    private void checkProximity()
    {
        // Initialize check parameters for below.
        var checkDepth = skinWidth;
        var origin = _raycastOrigins.bottomLeft;
        var size = new Vector2(boxCollider.bounds.size.x - skinWidth, checkDepth);
        var direction = Vector2.down;
        var checkDistance = skinWidth * 2;

        // Vertical checks.

        // Get to center x
        origin.x = transform.position.x;

        // Check below
        collision.below = Physics2D.BoxCast(origin, size, 0, direction, checkDistance, platformMask);

        // Check above
        origin.y = _raycastOrigins.topLeft.y;
        direction = Vector2.up;
        collision.above = Physics2D.BoxCast(origin, size, 0, direction, skinWidth, platformMask);

        // Horizontal checks.

        // Get to center y.
        origin.y = transform.position.y;

        // Adjust check size.
        size.x = checkDepth;
        size.y = boxCollider.bounds.size.y - skinWidth;

        // Check right.
        origin.x = _raycastOrigins.bottomRight.x;
        direction = Vector2.right;
        collision.right = Physics2D.BoxCast(origin, size, 0, direction, skinWidth, platformMask);

        // Check left
        origin.x = _raycastOrigins.bottomLeft.x;
        direction = Vector2.left;
        collision.left = Physics2D.BoxCast(origin, size, 0, direction, skinWidth, platformMask);
    }

	#endregion
}
