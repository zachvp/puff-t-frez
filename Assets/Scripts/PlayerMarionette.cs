﻿using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
public class PlayerMarionette :
    ICoreInput<HandGrenadeInput>,
    ICoreInput<PlayerInput>,
    ICoreInput<CombatHandInput>
{
	private readonly PlayerSkeleton skeleton;

	private Vector2 footTick;
    
	public PlayerMarionette() { }

	public PlayerMarionette(PlayerSkeleton playerSkeleton)
	{
		skeleton = playerSkeleton;
		skeleton.OnLimbAttached += HandleLimbAttached;

		skeleton.SetActive(Limb.GRENADE, false);

        FrameCounter.Instance.OnUpdate += HandleUpdate;
    }

    public void HandleUpdate(long count, float deltaTime)
    {
		if (skeleton.grenade.isGrabbable &&
			skeleton.body.entity.trigger.current.IsColliding(skeleton.grenade.entity))
        {
			skeleton.grenade.Grab();
            skeleton.hand.entity.SetPosition(skeleton.grenade.entity.Position);
            skeleton.SetActive(Limb.HAND, true);
            skeleton.SetActive(Limb.GRENADE, false);
        }
    }

	public void ApplyInput(InputSnapshot<PlayerInput> input)
	{
		// Now that we have the motor direction, we can adjust entity anchors
        // corresponding to the new direction.
		if (FlagsHelper.IsSet(input.held.direction.Flags, Direction2D.LEFT))
        {
			// Set hand anchor.
			skeleton.hand.SetRoot(skeleton.body.entity.handAnchorLeft);

			// Set foot anchor.
			skeleton.foot.SetRoot(skeleton.body.entity.footAnchorLeft);
        }
		else if (FlagsHelper.IsSet(input.held.direction.Flags, Direction2D.RIGHT))
        {
			// Set hand anchor
			skeleton.hand.SetRoot(skeleton.body.entity.handAnchorRight);

			// Set foot anchor.
			skeleton.foot.SetRoot(skeleton.body.entity.footAnchorRight);
        }

        // After making skeleton adjustments, apply input.
		skeleton.body.ApplyInput(input);

		// TODO: Move this to foot motor and handle with states.
		var bodyVelocity = skeleton.body.entity.velocity;
		var amplitude = 32;
		var interval = 0.12f;

		if (skeleton.body.entity.collision.current.state.Below)
		{
			if (bodyVelocity.x > 0)
			{
				var deltaPos = skeleton.foot.entity.Position - 
		                       skeleton.body.entity.footAnchorRight.position;
                                
				var footPos = skeleton.body.entity.footAnchorRight.position;
                
                footPos = skeleton.foot.entity.Position;

				footPos.x -= Mathf.Sin(footTick.x) * amplitude;
				footTick.x += interval;

				skeleton.foot.entity.SetPosition(footPos);
			}
			else if (bodyVelocity.x < 0)
			{
				var deltaPos = skeleton.foot.entity.Position -
		                       skeleton.body.entity.footAnchorLeft.position;
                
				var footPos = skeleton.body.entity.footAnchorLeft.position;

				footPos = skeleton.foot.entity.Position;

				footPos.x += Mathf.Sin(footTick.y) * amplitude;
				footTick.y += interval;

                skeleton.foot.entity.SetPosition(footPos);
			}
			else
			{
				footTick = Vector2.zero;
			}
		}
	}

	public void ApplyInput(InputSnapshot<HandGrenadeInput> input)
	{
		if (input.pressed.launch && !skeleton.IsActive(Limb.GRENADE, Logical.AND))
		{
			skeleton.grenade.entity.SetPosition(skeleton.hand.entity.Position);

            skeleton.SetActive(Limb.GRENADE, true);
            skeleton.SetActive(Limb.HAND, false);

			Physics2D.IgnoreCollision(skeleton.body.entity.collider, skeleton.grenade.entity.collider);

			var grenadeDirection = new CoreDirection(input.held.direction);

			if (grenadeDirection.IsEmptyHorizontal())
			{
				grenadeDirection = skeleton.body.direction;
			}

			grenadeDirection.Update(Direction2D.UP, true);
			grenadeDirection.Update(Direction2D.DOWN, false);

			skeleton.grenade.Launch(skeleton.body.entity.velocity, grenadeDirection);
        }
	}

	public void ApplyInput(InputSnapshot<CombatHandInput> input)
	{
		if (input.pressed.grab)
		{
			skeleton.combatHand.Punch(input.held.direction);
		}
	}

    // Handlers
	public void HandleLimbAttached(Limb skeletonLimbs, Limb attachedLimb)
    {
		if (FlagsHelper.IsSet(skeletonLimbs, Limb.HAND | Limb.BODY, Logical.AND))
		{
            skeleton.hand.entity.SetPosition(skeleton.body.entity.Position);
            skeleton.SetActive(Limb.HAND, true);
        }
    }
}
