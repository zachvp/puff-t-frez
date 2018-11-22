using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerMotor :
    Motor<PlayerMotorData, PlayerCharacterEntity>,
    ICoreInput<PlayerInput>
{
    // The direction of input.
	private InputSnapshot<PlayerInput> input;

    // The amount of frames the motor has been jumping.
    private int additiveJumpFrameCount;

    // How many times a jump has been performed.
    private int jumpCount;

    // The direction of the most recent wall jump.
    private CoreDirection wallJumpImpactDirection;
    
	private State state;

    public PlayerMotor(PlayerCharacterEntity pc,
	                   Transform t)
		: base(pc, t)
	{
		input = new InputSnapshot<PlayerInput>();
        wallJumpImpactDirection = new CoreDirection();

        data = ScriptableObject.CreateInstance<PlayerMotorData>();

		direction = data.initialDirection;

        FrameCounter.Instance.OnUpdate += HandleUpdate;
    }


    // todo: move this to handleInput
    public void HandleUpdate(long count, float deltaTime)
    {
        // Compute state from input

        // In update, need to compute all possible physical actions the player must perform in FixedUpdate
        if (entity.collision.current.state.Below)
        {
            HandleGrounded();
        }
        else
        {
            HandleNotGrounded();
        }

        if (FlagsHelper.IsSet(state, State.CROUCH))
        {
            HandleCrouch();
        }

        // At this point, all the motor's velocity computations are complete,
        // so we can determine the motor's direction.
        ComputeDirection(entity.velocity);
    }

    // IPlayerInput functions
	public void ApplyInput(InputSnapshot<PlayerInput> inputSnapshot)
	{
        input = inputSnapshot;
    }

    // IMotor functions
	// TODO: Make this a public Property like Position, etc in Motor super class
    public Vector3 GetVelocity()
	{
        return entity.velocity;
    }

    private void HandleGrounded()
	{
        var movement = input.held.direction.Vector;
        var resolvedVelocity = entity.velocity;

		FlagsHelper.Unset(ref state, State.JUMP);
        wallJumpImpactDirection.Clear();

        // Horizontal movement.
        resolvedVelocity.x = movement.x * data.velocityHorizontalGroundMax;

        if (!FlagsHelper.IsSet(state, State.CROUCH))
        {
            if (input.held.crouch)
            {
				var newBounds = entity.LocalScale;
                var crouchPosition = entity.Position;

                newBounds.x *= data.boundsMultiplierCrouchX;
                newBounds.y *= data.boundsMultiplierCrouchY;
                crouchPosition.y -= entity.LocalScale.y;

                var sizeOffset = CoreUtilities.GetWorldSpaceSize(newBounds, entity.collider as BoxCollider2D, 0.5f).x;
                var checkDistance = newBounds.x;
                var hitLeft = entity.Check(Constants.Directions.LEFT, checkDistance);
                var hitRight = entity.Check(Constants.Directions.RIGHT, checkDistance);

                if (hitLeft)
                {
                    crouchPosition.x = hitLeft.point.x + sizeOffset;
                }
                if (hitRight)
                {
                    crouchPosition.x = hitRight.point.x - sizeOffset;
                }

                entity.SetLocalScale(newBounds);
                entity.SetPosition(crouchPosition);
                
				FlagsHelper.Set(ref state, State.CROUCH);
            }
        }

        if (!input.held.jump)
        {
            additiveJumpFrameCount = 0;
            jumpCount = 0;
        }

        // Jump
        if (input.pressed.jump)
        {
            if (jumpCount < data.jumpCountMax)
            {
                resolvedVelocity.y = data.velocityJumpImpulse;
                jumpCount++;
            }
        }

        entity.SetVelocity(resolvedVelocity);
    }

    private void HandleNotGrounded()
	{
		var movement = input.held.direction.Vector;
        var resolvedVelocity = entity.velocity;

        // Air directional influence
        resolvedVelocity.x += movement.x * data.accelerationHorizontalAir;
        resolvedVelocity.x = Mathf.Clamp(resolvedVelocity.x, -data.velocityHorizontalAirMax, data.velocityHorizontalAirMax);

        // Check for wall jump.
        if (jumpCount > 0 && input.pressed.jump)
        {
            // Buffer collision state X frames
            // Check if .left is in buffer up to Y frames back
            // Wall jump direction check prevents motor from indefinitely climbing up the same wall.
            // Motor jump off the opposite wall for this to reset.
            var bufferedCollision = entity.GetBufferedCollisionState();

            if (Mathf.Abs(wallJumpImpactDirection.Vector.x) < 1 ||
                CoreDirection.IsOppositeHorizontal(wallJumpImpactDirection, bufferedCollision.direction))
            {
                if (Mathf.Abs(bufferedCollision.direction.Vector.x) > 0)
                {
                    wallJumpImpactDirection.Update(bufferedCollision.direction);
                    wallJumpImpactDirection.ClearVertical();

                    var velocityX = -bufferedCollision.direction.Vector.x * data.velocityWallJumpHorizontal;

                    if (CoreDirection.IsSameHorizontal(bufferedCollision.direction, input.held.direction))
                    {
                        // Zero out x velocity if input is in same direction as the collision side. Meant to help climbing up.
                        velocityX = 0;
                    }

                    resolvedVelocity.x = velocityX;
                    resolvedVelocity.y = data.velocityWallJumpVertical;

                    CallbackManager manager = new CallbackManager();
                }
            }
        }

        // Cut short the jump if the motor bumped something above.
        if (entity.collision.current.state.Above)
        {
            additiveJumpFrameCount = data.frameLimitJumpAdditive;
        }

        entity.SetVelocity(resolvedVelocity);
    }

    private void HandleCrouch()
	{
		if (!input.held.crouch)
        {
            var check = entity.CheckProximity(entity.LocalScale.y, Direction2D.UP);

            if (!check.Above)
            {
                var newBounds = entity.PriorTransform.localScale;
                var crouchPosition = entity.Position;

                crouchPosition.y += newBounds.y;

                entity.SetLocalScale(newBounds);
                entity.SetPosition(crouchPosition);
                FlagsHelper.Unset(ref state, State.CROUCH);
            }
            else
            {
                Debug.Log("detected something above");
            }
        }
	}

	[Flags]
	enum State
	{
		NONE      = 1 << 0,
		CROUCH    = 1 << 1,
		JUMP      = 1 << 2
	}
}
