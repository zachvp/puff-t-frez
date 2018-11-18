using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
// todo: this should detect collisions between entities and call approp methods in entities based on that
public class PlayerMarionette :
    ICoreInput<HandGrenadeInput>,
    ICoreInput<PlayerInput>
{
	private readonly PlayerSkeleton skeleton;

	private Vector2 footTick;
	private float t = 200000;
	float u = 0;
        
	public PlayerMarionette() { }

	public PlayerMarionette(PlayerSkeleton playerSkeleton)
	{
		skeleton = playerSkeleton;
		skeleton.OnLimbAttached += HandleLimbAttached;

		skeleton.grenade.OnGrab += HandleGrenadePickup;
		skeleton.SetActive(Limb.GRENADE, false);
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
		var bodyVelocity = skeleton.body.GetVelocity();
		var amplitude = 32;
		var interval = 0.12f;

		if (skeleton.body.entity.collision.current.state.Below)
		{
			if (bodyVelocity.x > 0)
			{
				var deltaPos = skeleton.foot.entity.Position - 
		                       skeleton.body.entity.footAnchorRight.position;

				//Debug.LogFormat("delta pos mag: {0}", deltaPos.magnitude);

				if (deltaPos.magnitude > 0)
				{
					t = Mathf.Min(t, deltaPos.magnitude);
				}

				u = Mathf.Max(u, deltaPos.magnitude);
                
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
		if (input.pressed.launch && !skeleton.IsActive(Limb.GRENADE))
		{
			var bodyData = new MotorData(skeleton.body.GetDirection(),
			                             skeleton.body.GetVelocity());

			input.held.data = bodyData;

			skeleton.grenade.ApplyInput(input);

			skeleton.SetActive(Limb.GRENADE, true);
			skeleton.SetActive(Limb.HAND, false);
			skeleton.grenade.entity.SetPosition(skeleton.hand.entity.Position);
		}
	}
    
	public void HandleGrenadePickup(CollisionContext context)
	{
		skeleton.hand.entity.SetPosition(skeleton.grenade.entity.Position);
		skeleton.grenade.Reset();

		skeleton.SetActive(Limb.HAND, true);
		skeleton.SetActive(Limb.GRENADE, false);
	}

    // Handlers
	public void HandleLimbAttached(Limb skeleton, Limb attachedLimb)
    {
		if (FlagsHelper.IsSet(skeleton, Limb.HAND | Limb.BODY, Logical.AND))
		{
			Reset();
		}
    }

    // Private
	private void Reset()
    {
		skeleton.hand.entity.SetPosition(skeleton.body.entity.Position);
		skeleton.SetActive(Limb.HAND, true);
    }
}
