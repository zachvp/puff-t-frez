﻿using UnityEngine;
using System;

public class PlayerMotor : Motor, IInputPlayerBody, IMotor
{
	private Entity entity;

    // Reference to the character controller engine.
    private CharacterController2D engine;

    // The motor velocity.
    private Vector2 velocity;

    // The direction of input.
    private PlayerInputSnapshot input;

    // The direction the motor is facing.
    private Vector2 motorDirection;

    // The configuration data driving movement and physics.
	private PlayerMotorData data;

    // The amount of frames the motor has been jumping.
    private int additiveJumpFrameCount;

    // How many times a jump has been performed.
    private int jumpCount;
    
    // The provided time between frames.
    private float deltaTime;

	private State state;
    
	public PlayerMotor(Entity playerEntity, CharacterController2D playerEngine)
	{
		input = new PlayerInputSnapshot();

		entity = playerEntity;
		engine = playerEngine;
		data = ScriptableObject.CreateInstance<PlayerMotorData>();

		// Define the initial motor direction to be facing right going down.
        motorDirection.x = 1;
        motorDirection.y = -1;

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	// When update is called, all input has been processed.
	public void HandleUpdate(int currentFrame, float deltaTime)
    {
        if (engine.isGrounded)
        {
            HandleGrounded();
        }
        else
        {
            HandleNotGrounded();
        }

		// Check all frames since jump was initiated for a release of the jump button.
        if (input.released.jump && jumpCount < data.jumpCountMax)
        {
            jumpCount++;
        }

        if (input.pressed.jump &&
            additiveJumpFrameCount < data.frameLimitJumpAdditive &&
            jumpCount < data.jumpCountMax)
        {
            ApplyJump();
        }
        
        // At this point, all the motor's velocity computations are complete,
        // so we can determine the motor's direction.
        ComputeMotorDirection();

        // Update the controller with the computed velocity.
        engine.Move(deltaTime * velocity);

		// TODO: Fix magic number
        if (Mathf.Abs(engine.velocity.y) < 0.01) {
            // Kind of a hack. The normally computed velocity is unreliable.
            // The only other case velocity is used is in handling slopes.
            velocity.y = 0;
        }
    }

    // IPlayerInput functions
    public void ApplyInput(PlayerInputSnapshot inputSnapshot) {
        input = inputSnapshot;
    }

    public void ApplyDeltaTime(float time) {
        deltaTime = time;
    }

    // IMotor functions
    public Vector3 GetVelocity() {
        return velocity;
    }

    public Vector3 GetDirection() {
        return motorDirection;
    }

    private void HandleGrounded() {
        var movement = input.pressed.movement;

		FlagsHelper.Unset(ref state, State.JUMP);
        
        // Horizontal movement.
        velocity.x = movement.x * data.velocityHorizontalGroundMax;

        // Reset jump states if jump isn't pressed.
        if (!input.pressed.jump) {
            additiveJumpFrameCount = 0;
            jumpCount = 0;
        }

		if (!FlagsHelper.IsSet(state, State.CROUCH))
        {
			if (input.pressed.crouch)
			{
				var newBounds = entity.localScale;
                var crouchPosition = entity.position;

				// TODO: Clean up magic values
				newBounds.x *= 1.5f;
                newBounds.y /= 2;
                crouchPosition.y -= entity.localScale.y;

                entity.SetLocalScale(newBounds);
                entity.SetPosition(crouchPosition);
				FlagsHelper.Set(ref state, State.CROUCH);
			}
        }
		else if (FlagsHelper.IsSet(state, State.CROUCH))
		{
			if (!input.pressed.crouch)
			{
				var check = engine.CheckProximity(entity.localScale.y, Direction2D.ABOVE);

                if (!check.above)
                {
                    var newBounds = entity.localScale;
                    var crouchPosition = entity.position;

					// TODO: Clean up magic numbers
					newBounds.x /= 1.5f;
                    newBounds.y *= 2;
                    crouchPosition.y += newBounds.y;

                    entity.SetLocalScale(newBounds);
                    entity.SetPosition(crouchPosition);
					FlagsHelper.Unset(ref state, State.CROUCH);
                }
			}
		}
    }

    private void HandleNotGrounded() {
        var movement = input.pressed.movement;

        // Motor is not grounded.
        // Air directional influence
        velocity.x += movement.x * data.accelerationHorizontalAir;

        // Clamp horizontal velocity so it doesn't get out of control.
        velocity.x = Mathf.Clamp(velocity.x, -data.velocityHorizontalAirMax, data.velocityHorizontalAirMax);

        // Check for wall collision in air, which should zero out x velocity.
        if (Mathf.Abs(input.pressed.movement.x) < 1 &&
           (engine.collision.right || engine.collision.left))
        {
            velocity.x = 0;
        }

        // Check for wall jump.
        if (jumpCount > 0 && input.pressed.jump)
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
        if (engine.collision.above)
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
        // Set the motor direction based on the velocty.
        // Motor direction should be 1 for positive velocity and -1 for
        // negative velocity.
        // Check for nonzero velocity
        if (Mathf.Abs(velocity.x) > 1)
        {
            motorDirection.x = velocity.x > 0 ? 1 : -1;
        }
        if (Mathf.Abs(velocity.y) > 1)
        {
            motorDirection.y = velocity.y > 0 ? 1 : -1;
        }

        Debug.AssertFormat((int) Mathf.Abs(motorDirection.x) == 1, "Motor X direction should always have a magnitude of one.");
        Debug.AssertFormat((int) Mathf.Abs(motorDirection.y) == 1, "Motor Y direction should always have a magnitude of one.");
    }

	[Flags]
	private enum State
	{
		NONE    = 1 << 0,
		CROUCH  = 1 << 1,
		JUMP    = 1 << 2
	}
}
