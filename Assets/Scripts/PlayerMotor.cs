using UnityEngine;
using System;

public class PlayerMotor : Motor<PlayerMotorData, PlayerCharacterEntity>, IInputPlayerBody, IMotor
{
    // Reference to the character controller engine.
    private CharacterController2D engine;

    // The direction of input.
	private InputSnapshot<PlayerInput> input;

    // The direction the motor is facing.
	private CoreDirection motorDirection;

    // The amount of frames the motor has been jumping.
    private int additiveJumpFrameCount;

    // How many times a jump has been performed.
    private int jumpCount;
    
    // The provided time between frames.
    private float deltaTime;

	private State state;
        
	public PlayerMotor(PlayerCharacterEntity pc,
	                   CharacterController2D e,
	                   Transform t)
		: base(pc, t)
	{
		input = new InputSnapshot<PlayerInput>();

		engine = e;
		data = ScriptableObject.CreateInstance<PlayerMotorData>();

		// TOOD: Move magic to data class
		motorDirection = new CoreDirection(new Vector2(1, -1));

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	// When update is called, all input has been processed.
	public void HandleUpdate(long currentFrame, float deltaTime)
    {
        if (engine.isGrounded)
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

		// Check all frames since jump was initiated for a release of the jump button.
        if (input.released.jump && jumpCount < data.jumpCountMax)
        {
            jumpCount++;
        }

		if (input.held.jump &&
            additiveJumpFrameCount < data.frameLimitJumpAdditive &&
            jumpCount < data.jumpCountMax)
        {
            ApplyJump();
        }
        
        // Update the controller with the computed velocity.
        engine.Move(deltaTime * velocity);

		if (Mathf.Abs(engine.velocity.y) < data.velocityThresholdMin)
		{
            // Kind of a hack. The normally computed velocity is unreliable.
            // The only other case velocity is used is in handling slopes.
            velocity.y = 0;
        }

		// At this point, all the motor's velocity computations are complete,
        // so we can determine the motor's direction.
        ComputeMotorDirection();
    }

    // IPlayerInput functions
	public void ApplyInput(InputSnapshot<PlayerInput> inputSnapshot)
	{
        input = inputSnapshot;
    }

    public void ApplyDeltaTime(float time)
	{
        deltaTime = time;
    }

    // IMotor functions
	// TODO: Make this a public Property like Position, etc in Motor super class
    public Vector3 GetVelocity()
	{
        return velocity;
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
        
        // Horizontal movement.
        velocity.x = movement.x * data.velocityHorizontalGroundMax;

        // Reset jump states if jump isn't pressed.
		if (!input.held.jump) {
            additiveJumpFrameCount = 0;
            jumpCount = 0;
        }

		if (!FlagsHelper.IsSet(state, State.CROUCH))
        {
			if (input.held.crouch)
			{
				var newBounds = entity.LocalScale;
                var crouchPosition = entity.Position;

				newBounds.x *= data.boundsMultiplierCrouchX;
				newBounds.y *= data.boundsMultiplierCrouchY;
                crouchPosition.y -= entity.LocalScale.y;

				var sizeOffset = CoreUtilities.GetWorldSpaceSize(newBounds, entity.Collider, 0.5f).x;
				var checkDistance = newBounds.x;
                var hitLeft = engine.CheckLeft(checkDistance, 1);
                var hitRight = engine.CheckRight(checkDistance, 1);
                
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
        velocity.x += movement.x * data.accelerationHorizontalAir;

        // Clamp horizontal velocity so it doesn't get out of control.
        velocity.x = Mathf.Clamp(velocity.x, -data.velocityHorizontalAirMax, data.velocityHorizontalAirMax);

		// Check for wall collision in air, which should zero out x velocity.
		var isNeutralInput = !FlagsHelper.IsSet(input.held.direction.Flags, Direction2D.LEFT | Direction2D.RIGHT);
        if (isNeutralInput && (engine.collision.Right || engine.collision.Left))
        {
            velocity.x = 0;
        }

		// Check for wall jump.
		if (jumpCount > 0 && input.held.jump)
        {
            // Buffer collision state X frames
            // Check if .left is in buffer up to Y frames back
            if (engine.IsCollisionBuffered(Direction2D.LEFT))
            {
                velocity.y = data.velocityWallJumpVertical;
                velocity.x = data.velocityWallJumpHorizontal;
            }
            if (engine.IsCollisionBuffered(Direction2D.RIGHT))
            {
                velocity.y = data.velocityWallJumpVertical;
                velocity.x = -data.velocityWallJumpHorizontal;
            }
        }

        // Cut short the jump if the motor bumped something above.
        if (engine.collision.Above)
        {
            additiveJumpFrameCount = data.frameLimitJumpAdditive;
            velocity.y = 0;
        }

        // Apply gravity if motor does not have jump immunity or if there is no
        // jump input.
        if (additiveJumpFrameCount > data.frameLimitJumpGravityImmunity ||
		    !FlagsHelper.IsSet(state, State.JUMP))
        {
            velocity.y -= data.gravity;
        }
    }

    private void HandleCrouch()
	{
		if (!input.held.crouch)
        {
            var check = engine.CheckProximity(entity.LocalScale.y, Direction2D.UP);

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
        // Initial jump push off the ground.
		if (additiveJumpFrameCount < 1)
		{
			if (engine.isGrounded)
			{
				FlagsHelper.Set(ref state, State.JUMP);
				velocity.y = data.velocityJumpImpulse;
				additiveJumpFrameCount++;
			}
        }
		else
		{
            velocity.y += Mathf.RoundToInt(data.velocityJumpMax / additiveJumpFrameCount);
			additiveJumpFrameCount++;
        }
    }

    private void ComputeMotorDirection()
    {
		var result = motorDirection.Vector;

        // Set the motor direction based on the velocty.
        // Motor direction should be 1 for positive velocity and -1 for
        // negative velocity.
        // Check for nonzero velocity
        if (Mathf.Abs(velocity.x) > 1)
        {
			result.x = velocity.x > 0 ? 1 : -1;
        }
        if (Mathf.Abs(velocity.y) > 1)
        {
			result.y = velocity.y > 0 ? 1 : -1;
        }
        
		motorDirection.Update(result);

		Debug.AssertFormat((int) Mathf.Abs(result.x) == 1 ||
		                   (int) Mathf.Abs(result.x) == 0,
		                   "Motor X direction should always have a magnitude of one.");
		Debug.AssertFormat((int) Mathf.Abs(result.y) == 1 ||
		                   (int) Mathf.Abs(result.y) == 0,
		                   "Motor Y direction should always have a magnitude of one.");
    }

	[Flags]
	private enum State
	{
		NONE    = 1 << 0,
		CROUCH  = 1 << 1,
		JUMP    = 1 << 2
	}
}
