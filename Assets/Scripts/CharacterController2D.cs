﻿#define DEBUG_CC2D_RAYS
using UnityEngine;
using System;
using System.Collections.Generic;

public class CharacterController2D
{
	#region events, properties and fields

	public event Action<RaycastHit2D> OnControllerCollidedEvent;

	public BoxCollider2D collider;
	public Rigidbody2D rigidBody;

	public CollisionState2D collision = new CollisionState2D();
	public Vector3 velocity;
    public bool isGrounded { get { return collision.Below; } }

	#endregion

	private PlayerEngineData data;

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

	private LinkedList<CollisionState2D> collisionBuffer;

	private Entity entity;

	#region internal types

    private struct CharacterRaycastOrigins
    {
        public Vector3 topLeft;
        public Vector3 bottomRight;
        public Vector3 bottomLeft;
    }

    #endregion

	public CharacterController2D(Entity entityInstance,
	                             BoxCollider2D colliderInstance,
	                             Rigidbody2D rigidbodyInstance)
    {
		collisionBuffer = new LinkedList<CollisionState2D>();
		data = ScriptableObject.CreateInstance<PlayerEngineData>();


		SetDependencies(entityInstance, colliderInstance, rigidbodyInstance);

		// add our one-way platforms to our normal platform mask so that we can land on them from above
		data.platformMask |= data.oneWayPlatformMask;

		RecalculateDistanceBetweenRays();
    }

	//[System.Diagnostics.Conditional( "DEBUG_CC2D_RAYS" )]
	private void DrawRay(Vector3 start, Vector3 dir, Color color)
	{
		Debug.DrawRay(start, dir, color);
	}


	#region Public

	public void SetDependencies(Entity entityInstance,
	                            BoxCollider2D colliderInstance,
	                            Rigidbody2D rigidbodyInstance)
	{
		entity = entityInstance;
		collider = colliderInstance;
		rigidBody = rigidbodyInstance;

		entity.OnScaleChange += HandleScaleChange;
	}

	public void HandleScaleChange(Vector3 scale)
	{
		// TODO: Number of rays should be based on scale.
		RecalculateDistanceBetweenRays();
	}

	public bool IsCollisionBuffered(Direction2D direction)
    {
        var result = false;

        foreach (CollisionState2D collisionState in collisionBuffer)
        {
			result |= FlagsHelper.IsSet(collisionState.direction.Flags, direction);
        }

        return result;
    }

	/// <summary>
	/// attempts to move the character to position + deltaMovement. Any colliders in the way will cause the movement to
	/// stop when run into.
	/// </summary>
	/// <param name="deltaMovement">Delta movement.</param>
	public void Move( Vector3 deltaMovement)
	{
        // save off our current grounded state which we will use for becameGroundedThisFrame
        var oldCollisionState = new CollisionState2D(collision);

		// clear our state
		collision.Reset();
		_raycastHitsThisFrame.Clear();
		_isGoingUpSlope = false;

		var desiredPosition = entity.Position + deltaMovement;
		primeRaycastOrigins( desiredPosition, deltaMovement );

		// first, we check for a slope below us before moving
		// only check slopes if we are going down and grounded
        if( deltaMovement.y < 0 && oldCollisionState.Below)
			handleVerticalSlope( ref deltaMovement );

		// now we check movement in the horizontal dir
        if( Mathf.Abs(deltaMovement.x) > 0 )
            moveHorizontally( ref deltaMovement, oldCollisionState );

		// next, check movement in the vertical dir
        if( Mathf.Abs(deltaMovement.y) > 0 )
            moveVertically( ref deltaMovement, oldCollisionState );

		// move then update our state
		if( data.usePhysicsForMovement )
		{
			rigidBody.MovePosition( entity.Position + deltaMovement );
			velocity = rigidBody.velocity;
		}
		else
		{
			//transform.Translate( deltaMovement, Space.World );
			var newPosition = entity.Position + deltaMovement;

			entity.SetPosition(newPosition);

			// only calculate velocity if we have a non-zero deltaTime
            if( FrameCounter.Instance.deltaTime > 0 ) {
				velocity = deltaMovement / FrameCounter.Instance.deltaTime;
            }
		}

		// After translation, update proximity check so collision state is fresh.
		// Without this, collision state doesn't get updated if there's no delta movement.
		collision = CheckProximity(data.skinWidth * 2, Direction2D.ALL);

		// Add to the collision buffer.
		collisionBuffer.AddFirst(new CollisionState2D(collision));

		if (collisionBuffer.Count > data.collisionBufferMax) {
			collisionBuffer.RemoveLast();
		}

		// set our becameGrounded state based on the previous and current collision state
        if( !oldCollisionState.Below && collision.Below )
			collision.becameGroundedThisFrame = true;

		// if we are going up a slope we artificially set a y velocity so we need to zero it out here
		if( _isGoingUpSlope )
			velocity.y = 0;

		// send off the collision events if we have a listener
		if( OnControllerCollidedEvent != null )
		{
			for( var i = 0; i < _raycastHitsThisFrame.Count; i++ )
				OnControllerCollidedEvent( _raycastHitsThisFrame[i] );
		}
	}


	/// <summary>
	/// moves directly down until grounded
	/// </summary>
	public void WarpToGrounded()
	{
		do
		{
            Move( new Vector3( 0, -1f, 0 ) );
		} while( !isGrounded );
	}


	/// <summary>
	/// this should be called anytime you have to modify the BoxCollider2D at runtime. It will recalculate the distance between the rays used for collision detection.
	/// It is also used in the skinWidth setter in case it is changed at runtime.
	/// </summary>
	public void RecalculateDistanceBetweenRays()
	{
		// figure out the distance between our rays in both directions
		// horizontal
		var colliderUseableHeight = collider.size.y * Mathf.Abs( entity.LocalScale.y ) - ( 2f * data.skinWidth );
		_verticalDistanceBetweenRays = colliderUseableHeight / ( data.totalHorizontalRays - 1 );

		// vertical
		var colliderUseableWidth = collider.size.x * Mathf.Abs( entity.LocalScale.x ) - ( 2f * data.skinWidth );
		_horizontalDistanceBetweenRays = colliderUseableWidth / ( data.totalVerticalRays - 1 );
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
		var modifiedBounds = collider.bounds;
		modifiedBounds.Expand( -2f * data.skinWidth );

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
    private void moveHorizontally( ref Vector3 deltaMovement, CollisionState2D oldCollisionState )
	{
		var isGoingRight = deltaMovement.x > 0;
		var rayDistance = Mathf.Abs( deltaMovement.x ) + data.skinWidth;
		var rayDirection = isGoingRight ? Vector2.right : -Vector2.right;
		var initialRayOrigin = isGoingRight ? _raycastOrigins.bottomRight : _raycastOrigins.bottomLeft;

		for( var i = 0; i < data.totalHorizontalRays; i++ )
		{
			var ray = new Vector2( initialRayOrigin.x, initialRayOrigin.y + i * _verticalDistanceBetweenRays );

			DrawRay( ray, rayDirection * rayDistance, Color.red );

			// if we are grounded we will include oneWayPlatforms only on the first ray (the bottom one). this will allow us to
			// walk up sloped oneWayPlatforms
            if( i == 0 && oldCollisionState.Below) {
				_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, data.platformMask );
			} else {
				_raycastHit = Physics2D.Raycast( ray, rayDirection, rayDistance, data.platformMask & ~data.oneWayPlatformMask);
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
					deltaMovement.x -= data.skinWidth;
					collision.Right = true;
				}
				else
				{
					deltaMovement.x += data.skinWidth;
					collision.Left = true;
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
		if( angle < data.slopeLimit )
		{
			// we only need to adjust the deltaMovement if we are not jumping
			// TODO: this uses a magic number which isn't ideal!
			if( deltaMovement.y < data.jumpingThreshold )
			{
				// apply the slopeModifier to slow our movement up the slope
				var slopeModifier = data.slopeSpeedMultiplier.Evaluate( angle );
				deltaMovement.x *= slopeModifier;

				// we dont set collisions on the sides for this since a slope is not technically a side collision

				// smooth y movement when we climb. we make the y movement equivalent to the actual y location that corresponds
				// to our new x location using our good friend Pythagoras
				deltaMovement.y = Mathf.Abs( Mathf.Tan( angle * Mathf.Deg2Rad ) * deltaMovement.x );
				_isGoingUpSlope = true;

				collision.Below = true;
			}
		}
		else // too steep. get out of here
		{
			deltaMovement.x = 0;
		}

		return true;
	}

    private void moveVertically( ref Vector3 deltaMovement, CollisionState2D oldCollisionState )
	{
		var isGoingUp = deltaMovement.y > 0;
		var rayDistance = Mathf.Abs( deltaMovement.y ) + data.skinWidth;
		var rayDirection = isGoingUp ? Vector2.up : -Vector2.up;
		var initialRayOrigin = isGoingUp ? _raycastOrigins.topLeft : _raycastOrigins.bottomLeft;

		// apply our horizontal deltaMovement here so that we do our raycast from the actual position we would be in if we had moved
		initialRayOrigin.x += deltaMovement.x;

		// if we are moving up, we should ignore the layers in oneWayPlatformMask
		var mask = data.platformMask;
        if( isGoingUp && !oldCollisionState.Below)
			mask &= ~data.oneWayPlatformMask;

		for( var i = 0; i < data.totalVerticalRays; i++ )
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
					deltaMovement.y -= data.skinWidth;
					collision.Above = true;
				}
				else
				{
					deltaMovement.y += data.skinWidth;
					collision.Below = true;
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
		var slopeCheckRayDistance = data.slopeLimitTangent * ( _raycastOrigins.bottomRight.x - centerOfCollider );

		var slopeRay = new Vector2( centerOfCollider, _raycastOrigins.bottomLeft.y );
		DrawRay( slopeRay, rayDirection * slopeCheckRayDistance, Color.yellow );
		_raycastHit = Physics2D.Raycast( slopeRay, rayDirection, slopeCheckRayDistance, data.platformMask );
		if( _raycastHit )
		{
			// bail out if we have no slope
			var angle = Vector2.Angle( _raycastHit.normal, Vector2.up );
			if(Mathf.RoundToInt(angle) == 0)
			{
				return;
			}

			// we are moving down the slope if our normal and movement direction are in the same x direction
			var isMovingDownSlope = Mathf.Sign( _raycastHit.normal.x ) == Mathf.Sign( deltaMovement.x );
			if( isMovingDownSlope )
			{
				// going down we want to speed up in most cases so the slopeSpeedMultiplier curve should be > 1 for negative angles
				var slopeModifier = data.slopeSpeedMultiplier.Evaluate( -angle );
				deltaMovement.y = _raycastHit.point.y - slopeRay.y - data.skinWidth;
				deltaMovement.x *= slopeModifier;
				collision.movingDownSlope = true;
				collision.slopeAngle = angle;
			}
		}
	}

	public CollisionState2D CheckProximity(float distance, Direction2D mask)
    {
		var proximityCollision = new CollisionState2D();

		// Check below
		if (FlagsHelper.IsSet(mask, Direction2D.DOWN))
		{
			proximityCollision.Below = CheckBelow(distance, data.skinWidth);
		}

        // Check above
		if (FlagsHelper.IsSet(mask, Direction2D.UP))
		{
			proximityCollision.Above = CheckAbove(distance, data.skinWidth);
		}        

        // Check right.
		if (FlagsHelper.IsSet(mask, Direction2D.RIGHT))
		{
			proximityCollision.Right = CheckRight(distance, data.skinWidth);
		}

        // Check left
		if (FlagsHelper.IsSet(mask, Direction2D.LEFT))
		{
			proximityCollision.Left = CheckLeft(distance, data.skinWidth);
		}

		return proximityCollision;
    }

	public RaycastHit2D CheckBelow(float distance, float thiccness)
	{
		var origin = new Vector2(entity.Position.x, _raycastOrigins.bottomLeft.y);
		var size = new Vector2(collider.bounds.size.x - data.skinWidth, thiccness);

		return Physics2D.BoxCast(origin,
		                         size,
		                         0,
		                         Vector2.down,
		                         distance,
		                         data.platformMask);
	}

	public RaycastHit2D CheckAbove(float distance, float thiccness)
    {
		var origin = new Vector2(entity.Position.x, _raycastOrigins.topLeft.y);
        var size = new Vector2(collider.bounds.size.x - data.skinWidth, thiccness);

		return Physics2D.BoxCast(origin,
		                         size,
		                         0,
		                         Vector2.up,
		                         distance,
		                         data.platformMask);
    }

	public RaycastHit2D CheckRight(float distance, float thiccness)
    {
		var origin = new Vector2(_raycastOrigins.bottomRight.x, entity.Position.y);
		var size = new Vector2(thiccness, collider.bounds.size.y - data.skinWidth);

		return Physics2D.BoxCast(origin,
		                         size,
		                         0,
		                         Vector2.right,
		                         distance,
		                         data.platformMask);
    }

	public RaycastHit2D CheckLeft(float distance, float thiccness)
    {
		var origin = new Vector2(_raycastOrigins.bottomLeft.x, entity.Position.y);
		var size = new Vector2(thiccness, collider.bounds.size.y - data.skinWidth);

		return Physics2D.BoxCast(origin,
                                 size,
                                 0,
		                         Vector2.left,
                                 distance,
                                 data.platformMask);
    }

	#endregion
}
