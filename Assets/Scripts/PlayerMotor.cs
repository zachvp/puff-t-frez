﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(
        typeof(CharacterController2D)
    )
]
public class PlayerMotor : MonoBehaviour, IPlayerInput
{
    // TODO: Implement int Vector2?

    // Reference to the character controller engine.
    private CharacterController2D engine;

    // The motor velocity.
    private Vector2 velocity;

    // The direction of input.
    private PlayerInput inputDirection;

    private PlayerInput inputReleaseDirection;

    // The direction the motor is facing.
    private Vector2 motorDirection;

    private PlayerMotorData motorData;

    private int additiveJumpFrameCount;

    private int jumpCount;

    private int jumpFrameStart;

    private float deltaTime;

    private float maxHeight;

    // TODO: Move game logic to separate class (when can wall jump)

    public void Awake()
    {
        inputDirection = new PlayerInput();
        inputReleaseDirection = new PlayerInput();

        engine = GetComponent<CharacterController2D>();
        motorData = ScriptableObject.CreateInstance<PlayerMotorData>();
    }

	public void Start()
	{
        engine.warpToGrounded();
	}

	// When update is called, all input has been processed.
	public void Update()
    {
        var movement = inputDirection.movement;

        maxHeight = Mathf.Max(maxHeight, transform.position.y);

        //Debug.LogFormat("Max height: {0}", maxHeight);

        if (engine.isGrounded)
        {
            // Horizontal movement.
            velocity.x = movement.x * motorData.velocityHorizontalGroundMax;

            // Reset jump states.
            additiveJumpFrameCount = 0;
            jumpCount = 0;

            if (movement.y < 0)
            {
                // TODO: Perform crouch
            }
        }
        else
        {
            // Motor is not grounded.
            // Air directional influence
            //velocity.x += inputDirection.x * motorData.accelerationHorizontalAir;

            // Clamp horizontal velocity so it doesn't get out of control.
            //velocity.x = Mathf.Clamp(velocity.x, -motorData.velocityHorizontalAirMax, motorData.velocityHorizontalAirMax);

            // Check for wall jump.
            // TODO: should check jump flag instead of movement axis
            if (jumpCount > 0 && movement.y > 0)
            {
                var proximityCollisionState = engine.getProximityCollisionState();

                // TODO: Move magic numbers to motor data
                // TODO: Proximity collision state should use same mechanisms as grounded.
                if (proximityCollisionState.left)
                {
                    //velocity.y = 900;
                    velocity.x = 100;
                }

                if (proximityCollisionState.right)
                {
                    //velocity.y = 900;
                    velocity.x = -100;
                }
            }

            // Apply gravity if motor does not have jump immunity.
            //if (additiveJumpFrameCount > motorData.frameLimitJumpGravityImmunity || inputDirection.y < 1)
            {
                velocity.y -= motorData.gravity;
            }
        }

        // Check all frames since jump was initiated for a release of the jump button.
        if (inputReleaseDirection.movement.y > 0 && jumpCount < motorData.jumpCountMax)
        {
            jumpCount++;

            Debug.LogFormat("will jump if on ground");

            if (engine.isGrounded)
            {
                Debug.LogFormat("apply jump");
                velocity.y = 800;
            }
        }

        // TODO: Figure out jump inconsistencies.
        // ^ Maybe keep track of gravityApplied frames and make sure it equals
        // ^ the additive jump frames
        // Additive jump. The longer the jump input, the higher the jump, for a certain amount of frames.
        if (movement.y > 0 && additiveJumpFrameCount < motorData.frameLimitJumpAdditive && velocity.y < motorData.velocityJumpMax)
        {
            // Initial jump push off the ground.
            if (additiveJumpFrameCount < 1)
            {
                jumpFrameStart = FrameCounter.Instance.count;
                //velocity.y += motorData.velocityJumpImpulse;
            }

            //addVelocity = Mathf.Max(addVelocity, 0);

            //velocity.y += motorData.velocityJumpAdditive;
            //velocity.y = Mathf.Clamp(velocity.y, 0, motorData.velocityJumpMax);
            //Debug.LogFormat("Y velocity: {0}", velocity.y);
            additiveJumpFrameCount++;
        }

        // Set the motor direction based on the velocty.
        // TODO: Can be moved to subroutine
        // Check for nonzero velocity
        if (Mathf.Abs(velocity.x) > 1)
        {
            // Motor direction should be 1 for positive velocity and 0 for negative velocity.
            motorDirection.x = velocity.x > 0 ? 1 : -1;
        }
        if (Mathf.Abs(velocity.y) > 1)
        {
            motorDirection.y = velocity.y > 0 ? 1 : -1;
        }

        // Update the controller with the computed velocity.
        engine.move(deltaTime * velocity);

        // Update the motor's velocity reference to the computed velocity.
        //velocity = engine.velocity;
        //      Debug.LogFormat ("Velocity: {0}", velocity);

        if (jumpCount > 0)
        {
            Debug.LogFormat("Y velocity: {0}", velocity.y);
        } else {
            Debug.LogFormat("jump count: {0}", jumpCount);
        }
    }

    // Input functions
    public void ApplyInput(PlayerInput input)
    {
        inputDirection = input;
    }

    public void ApplyInputRelease(PlayerInput inputRelease) {
        inputReleaseDirection = inputRelease;
    }

    public void ApplyDeltaTime(float time)
    {
        deltaTime = time;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
}
