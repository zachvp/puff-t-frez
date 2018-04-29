using UnityEngine;

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
    public Vector2 velocity;

    // The direction of input.
    private PlayerInputSnapshot input;

    // The direction the motor is facing.
    private Vector2 motorDirection;

    private PlayerMotorData motorData;

    private int additiveJumpFrameCount;

    private int jumpCount;

    private float deltaTime;

    // TODO: Move game logic to separate class (when can wall jump)

    public void Awake() {
        input = new PlayerInputSnapshot();

        engine = GetComponent<CharacterController2D>();
        motorData = ScriptableObject.CreateInstance<PlayerMotorData>();
    }

	public void Start() {
        //engine.warpToGrounded();
	}

	// When update is called, all input has been processed.
	public void Update()
    {
        if (engine.isGrounded) {
            //Debug.LogFormat("Is GROUNDED");
            HandleGrounded();
        } else {
            //Debug.LogFormat("NOT GROUNDED");
            HandleNotGrounded();
        }

        // Check all frames since jump was initiated for a release of the jump button.
        if (input.released.jump && jumpCount < motorData.jumpCountMax) {
            jumpCount++;
        }

        if (input.pressed.jump &&
            additiveJumpFrameCount < motorData.frameLimitJumpAdditive &&
            jumpCount < motorData.jumpCountMax) {
                ApplyJump();
        }

        if (engine.collision.right || engine.collision.left) {
            velocity.x = 0;
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

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    private void HandleGrounded() {
        Debug.LogFormat("GROUNDED");
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
        Debug.LogFormat("NOT GROUNDED");
        var movement = input.pressed.movement;

        // Motor is not grounded.
        // Air directional influence
        velocity.x += movement.x * motorData.accelerationHorizontalAir;

        // Clamp horizontal velocity so it doesn't get out of control.
        velocity.x = Mathf.Clamp(velocity.x, -motorData.velocityHorizontalAirMax, motorData.velocityHorizontalAirMax);

        // Check for wall jump.
        if (jumpCount > 0 && input.pressed.jump) {
            if (engine.collision.left) {
                velocity.y = motorData.velocityWallJumpVertical;
                velocity.x = motorData.velocityWallJumpHorizontal;
            }

            if (engine.collision.right) {
                velocity.y = motorData.velocityWallJumpVertical;
                velocity.x = -motorData.velocityWallJumpHorizontal;
            }
        }

        // Apply gravity if motor does not have jump immunity or if there is no
        // jump input.
        if (additiveJumpFrameCount > motorData.frameLimitJumpGravityImmunity || !input.pressed.jump) {
            velocity.y -= motorData.gravity;
        }

        if (engine.collision.above) {
            velocity.y = 0;
        }
    }

    // Additive jump. The longer the jump input, the higher the jump, for a
    // certain amount of frames.
    private void ApplyJump() {
        // Initial jump push off the ground.
        if (additiveJumpFrameCount < 1) {
            velocity.y += motorData.velocityJumpImpulse;
        }

        velocity.y += motorData.velocityJumpAdditive;
        additiveJumpFrameCount++;
    }

    private void ComputeMotorDirection() {
        // Set the motor direction based on the velocty.
        // Check for nonzero velocity
        if (Mathf.Abs(velocity.x) > 1) {
            // Motor direction should be 1 for positive velocity and 0 for
            // negative velocity.
            motorDirection.x = velocity.x > 0 ? 1 : -1;
        }

        if (Mathf.Abs(velocity.y) > 1) {
            motorDirection.y = velocity.y > 0 ? 1 : -1;
        }
    }
}
