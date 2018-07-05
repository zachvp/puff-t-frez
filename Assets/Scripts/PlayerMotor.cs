using UnityEngine;

public class PlayerMotor : IPlayerInput, IMotor
{
    // Reference to the character controller engine.
    private CharacterController2D engine;

    // The motor velocity.
    private Vector2 velocity;

    // The direction of input.
    private PlayerInputSnapshot input;

    // The direction the motor is facing.
    private Vector2 motorDirection;

    // The configuration data driving movement and physics.
    private PlayerMotorData motorData;

    // The amount of frames the motor has been jumping.
    private int additiveJumpFrameCount;

    // How many times a jump has been performed.
    private int jumpCount;

    // The provided time between frames.
    private float deltaTime;

    // TODO: Move game logic to separate class (when can wall jump)
	public PlayerMotor(CharacterController2D playerEngine)
	{
		input = new PlayerInputSnapshot();

		engine = playerEngine;
		motorData = ScriptableObject.CreateInstance<PlayerMotorData>();

		// Define the initial motor direction to be facing right going down.
        motorDirection.x = 1;
        motorDirection.y = -1;

		FrameCounter.Instance.OnStart += HandleStart;
		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public void HandleStart()
    {
        engine.warpToGrounded();
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
        if (input.released.jump && jumpCount < motorData.jumpCountMax)
        {
            jumpCount++;
        }

        if (input.pressed.jump &&
            additiveJumpFrameCount < motorData.frameLimitJumpAdditive &&
            jumpCount < motorData.jumpCountMax)
        {
            ApplyJump();
        }

        // At this point, all the motor's velocity computations are complete,
        // so we can determine the motor's direction.
        ComputeMotorDirection();

        // Update the controller with the computed velocity.
        engine.move(deltaTime * velocity);

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
        //Debug.LogFormat("GROUNDED");
        var movement = input.pressed.movement;

        // Horizontal movement.
        velocity.x = movement.x * motorData.velocityHorizontalGroundMax;

        // Reset jump states if jump isn't pressed.
        if (!input.pressed.jump) {
            additiveJumpFrameCount = 0;
            jumpCount = 0;
        }

        // TODO: This should be tied to crouch input.
        if (movement.y < 0)
        {
            // TODO: Perform crouch
        }
    }

    private void HandleNotGrounded() {
        //Debug.LogFormat("NOT GROUNDED");
        var movement = input.pressed.movement;

        // Motor is not grounded.
        // Air directional influence
        velocity.x += movement.x * motorData.accelerationHorizontalAir;

        // Clamp horizontal velocity so it doesn't get out of control.
        velocity.x = Mathf.Clamp(velocity.x, -motorData.velocityHorizontalAirMax, motorData.velocityHorizontalAirMax);

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
            if (engine.isCollisionBuffered(Direction2D.LEFT))
            {
                velocity.y = motorData.velocityWallJumpVertical;
                velocity.x = motorData.velocityWallJumpHorizontal;
            }
            if (engine.isCollisionBuffered(Direction2D.RIGHT))
            {
                velocity.y = motorData.velocityWallJumpVertical;
                velocity.x = -motorData.velocityWallJumpHorizontal;
            }
        }

        // Cut short the jump if the motor bumped something above.
        if (engine.collision.above)
        {
            additiveJumpFrameCount = motorData.frameLimitJumpAdditive;
            velocity.y = 0;
        }

        // Apply gravity if motor does not have jump immunity or if there is no
        // jump input.
        if (additiveJumpFrameCount > motorData.frameLimitJumpGravityImmunity ||
            !input.pressed.jump)
        {
            velocity.y -= motorData.gravity;
        }
    }

    // Additive jump. The longer the jump input, the higher the jump, for a
    // certain amount of frames.
    private void ApplyJump()
    {
        // Initial jump push off the ground.
        if (additiveJumpFrameCount < 1) {
            velocity.y = motorData.velocityJumpImpulse;
        } else {
            velocity.y += Mathf.RoundToInt(motorData.velocityJumpMax / additiveJumpFrameCount);
        }

        additiveJumpFrameCount++;
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
}
