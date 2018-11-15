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
    // todo: should be map of actions to PhysicsInput (rip actions outta there)
    PhysicsInput physicsInput;


    // todo: remove e param    
    public PlayerMotor(PlayerCharacterEntity pc,
	                   Transform t,
	                   CharacterController2D e)
		: base(pc, t)
	{
		input = new InputSnapshot<PlayerInput>();
        wallJumpDirection = new CoreDirection();
        physicsInput = new PhysicsInput();

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

        // At this point, all the motor's velocity computations are complete,
        // so we can determine the motor's direction.
        ComputeMotorDirection();
    }

	// When update is called, all input has been processed.
	public void HandleFixedUpdate(float deltaTime)
    {
        if (physicsInput.actions.Contains(Action.JUMP))
        {
            ApplyJump();
            physicsInput.actions.Remove(Action.JUMP);
        }

        if (physicsInput.actions.Contains(Action.WALL_JUMP))
        {
            ApplyWallJump();
            physicsInput.actions.Remove(Action.WALL_JUMP);
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
                physicsInput.actions.Add(Action.JUMP);
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

        if (!input.held.jump)
        {
            additiveJumpFrameCount = 0;
            jumpCount = 0;
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
        if (jumpCount > 0 && input.pressed.jump)
        {
            // Buffer collision state X frames
            // Check if .left is in buffer up to Y frames back
            // Wall jump direction check prevents motor from indefinitely climbing up the same wall.
            // Motor jump off the opposite wall for this to reset.
            Debug.Log("jump conditions met");

            physicsInput.bufferedCollisionState = entity.GetBufferedCollisionState();

            // todo: cleanup; bug if player goes between thin wall section
            if (Mathf.Abs(wallJumpDirection.Vector.x) < 1 ||
                CoreDirection.IsOppositeHorizontal(wallJumpDirection, physicsInput.bufferedCollisionState.direction))
            {
                if (Mathf.Abs(physicsInput.bufferedCollisionState.direction.Vector.x) > 0 ||
                    physicsInput.bufferedCollisionState.direction.IsSimultaneousHorizontal())
                {
                    physicsInput.actions.Add(Action.WALL_JUMP);
                    physicsInput.controlInput = new InputSnapshot<PlayerInput>(input);

                    wallJumpDirection.Update(physicsInput.bufferedCollisionState.direction);
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
        var velocityX = -physicsInput.bufferedCollisionState.direction.Vector.x * data.velocityWallJumpHorizontal;

        if (CoreDirection.IsSameHorizontal(physicsInput.bufferedCollisionState.direction, physicsInput.controlInput.held.direction))
        {
            // Zero out x velocity if input is in same direction as the collision side. Meant to help climbing up.
            velocityX = 0;
        }

        entity.SetVelocity(velocityX, data.velocityWallJumpVertical);
    }

    private void ComputeMotorDirection()
    {
        // todo: restore impl from master branch
		motorDirection.Update(entity.velocity);
    }

	[Flags]
	enum State
	{
		NONE    = 1 << 0,
		CROUCH  = 1 << 1,
		JUMP    = 1 << 2
	}

    enum Action
    {
        JUMP,
        WALL_JUMP
    }

    class PhysicsInput
    {
        public HashSet<Action> actions;
        public CollisionState2D bufferedCollisionState;
        public InputSnapshot<PlayerInput> controlInput;

        public PhysicsInput()
        {
            actions = new HashSet<Action>();
            bufferedCollisionState = new CollisionState2D();
            controlInput = new InputSnapshot<PlayerInput>();
        }
    }
}
