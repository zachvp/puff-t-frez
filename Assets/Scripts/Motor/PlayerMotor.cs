using UnityEngine;
using System;
using System.Collections.Generic;

public class PlayerMotor :
    Motor<PlayerMotorData, PlayerCharacterEntity>,
    ICoreInput<PlayerInput>, IMotor
{
    // The direction of input.
	private InputSnapshot<PlayerInput> input;

    // The direction the motor is facing.
	private CoreDirection motorDirection;

    // The amount of frames the motor has been jumping.
    private int additiveJumpFrameCount;

    // How many times a jump has been performed.
    private int jumpCount;

    // The direction of the most recent wall jump.
    private CoreDirection wallJumpDirection;
    
	private State state;

    // Physical actions to perform in FixedUpdate
    private HashSet<Action> actions;


    // todo: remove e param    
    public PlayerMotor(PlayerCharacterEntity pc,
	                   Transform t,
	                   CharacterController2D e)
		: base(pc, t)
	{
		input = new InputSnapshot<PlayerInput>();
        wallJumpDirection = new CoreDirection();
        actions = new HashSet<Action>();

        data = ScriptableObject.CreateInstance<PlayerMotorData>();

		// TOOD: Move magic to data class
		motorDirection = data.initialDirection;

        FrameCounter.Instance.OnUpdate += HandleUpdate;
        FrameCounter.Instance.OnFixedUpdate += HandleFixedUpdate;
    }

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

        if (!input.held.jump)
        {
            additiveJumpFrameCount = 0;
            jumpCount = 0;
        }

        // At this point, all the motor's velocity computations are complete,
        // so we can determine the motor's direction.
        ComputeMotorDirection();
    }

	// When update is called, all input has been processed.
	public void HandleFixedUpdate(float deltaTime)
    {
        if (actions.Contains(Action.JUMP))
        {
            ApplyJump();
            actions.Remove(Action.JUMP);
        }
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

	// TODO: Make this a public Property like Position, etc in Motor super class
	public CoreDirection GetDirection()
	{
        return motorDirection;
    }

    private void HandleGrounded()
	{
        var movement = input.held.direction.Vector;

		FlagsHelper.Unset(ref state, State.JUMP);
        wallJumpDirection.Clear();

        // Jump
        if (input.pressed.jump)
        {
            if (jumpCount < data.jumpCountMax)
            {
                actions.Add(Action.JUMP);
                jumpCount++;
            }
        }

        // Horizontal movement.
        entity.SetVelocity(movement.x * data.velocityHorizontalGroundMax, entity.velocity.y);

        if (!FlagsHelper.IsSet(state, State.CROUCH))
        {
            if (input.held.crouch)
            {
                var newBounds = entity.LocalScale;
                var crouchPosition = entity.Position;

                newBounds.x *= data.boundsMultiplierCrouchX;
                newBounds.y *= data.boundsMultiplierCrouchY;
                crouchPosition.y -= entity.LocalScale.y;

                var sizeOffset = CoreUtilities.GetWorldSpaceSize(newBounds, entity.collider, 0.5f).x;
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
    }

    private void HandleNotGrounded()
	{
		var movement = input.held.direction.Vector;

        // Motor is not grounded.
        // Air directional influence
        var v = entity.velocity;

        v.x += movement.x * data.accelerationHorizontalAir;
        v.x = Mathf.Clamp(v.x, -data.velocityHorizontalAirMax, data.velocityHorizontalAirMax);
        entity.SetVelocity(v);

        // Check for wall jump.
        if (jumpCount > 0 && input.held.jump)
        {
            // Buffer collision state X frames
            // Check if .left is in buffer up to Y frames back
            // Wall jump direction check prevents motor from indefinitely climbing up the same wall.
            // Motor jump off the opposite wall for this to reset.
            if (Mathf.Abs(wallJumpDirection.Vector.x) < 1)
            {
                var bufferedCollision = entity.GetBufferedCollisionState();

                if (Mathf.Abs(bufferedCollision.direction.Vector.x) > 0)
                {
                    var velocityX = -bufferedCollision.direction.Vector.x * data.velocityWallJumpHorizontal;

                    entity.SetVelocity(velocityX, data.velocityWallJumpVertical);
                    wallJumpDirection.Update(bufferedCollision.direction);
                    wallJumpDirection.ClearVertical();
                }
            }
        }

        // Cut short the jump if the motor bumped something above.
        if (entity.collision.current.state.Above)
        {
            additiveJumpFrameCount = data.frameLimitJumpAdditive;
        }
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
        }
	}

    // Additive jump. The longer the jump input, the higher the jump, for a
    // certain amount of frames.
    private void ApplyJump()
    {
        entity.SetVelocity(entity.velocity.x, data.velocityJumpImpulse);
    }

    private void ApplyWallJump()
    {

    }

    private void ComputeMotorDirection()
    {
        // todo: restore impl from master branch
		motorDirection.Update(entity.velocity);
    }

	[Flags]
	private enum State
	{
		NONE    = 1 << 0,
		CROUCH  = 1 << 1,
		JUMP    = 1 << 2
	}

    private enum Action
    {
        JUMP
    }
}
