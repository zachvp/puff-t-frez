// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
public class PlayerMarionette :
    ICoreInput<HandGrenadeInput>,
    ICoreInput<PlayerInput>,
    ICoreInput<HandInput>
{
	private readonly PlayerSkeleton skeleton;
    
	public PlayerMarionette() { }

	public PlayerMarionette(PlayerSkeleton playerSkeleton)
	{
		skeleton = playerSkeleton;
		skeleton.OnLimbAttached += HandleLimbAttached;

		skeleton.grenade.OnGrab += HandleGrenadePickup;
		skeleton.Activate(Limb.GRENADE, false);
	}

	public void ApplyInput(InputSnapshot<PlayerInput> input)
	{
		// Now that we have the motor direction, we can adjust entity anchors
        // corresponding to the new direction.
        if (FlagsHelper.IsSet(input.pressed.direction.Flags, Direction2D.LEFT))
        {
			UnityEngine.Debug.Log("pressed left - adjust anchors");
			skeleton.body.entity.handAnchorLeft.gameObject.SetActive(true);
			skeleton.body.entity.handAnchor.gameObject.SetActive(false);

			skeleton.hand.SetRoot(skeleton.body.entity.handAnchorLeft);
        }
        else if (FlagsHelper.IsSet(input.pressed.direction.Flags, Direction2D.RIGHT))
        {
			UnityEngine.Debug.Log("pressed right - adjust anchors");
			skeleton.body.entity.handAnchorLeft.gameObject.SetActive(false);
			skeleton.body.entity.handAnchor.gameObject.SetActive(true);

			skeleton.hand.SetRoot(skeleton.body.entity.handAnchor);
        }

        // After making skeleton adjustments, apply input.
		skeleton.body.ApplyInput(input);
	}

	public void ApplyInput(InputSnapshot<HandInput> input)
	{
		skeleton.hand.ApplyInput(input);
	}

	public void ApplyInput(InputSnapshot<HandGrenadeInput> input)
	{
		if (input.pressed.launch && !skeleton.IsActive(Limb.GRENADE))
		{
			var bodyData = new MotorData(skeleton.body.GetDirection(),
			                             skeleton.body.GetVelocity());

			input.held.data = bodyData;

			skeleton.grenade.ApplyInput(input);

			skeleton.Activate(Limb.GRENADE, true);
			skeleton.Activate(Limb.HAND, false);
			skeleton.grenade.entity.SetPosition(skeleton.hand.entity.Position);
		}
	}
    
    public void HandleGrenadePickup()
	{
		skeleton.hand.entity.SetPosition(skeleton.grenade.entity.Position);
		skeleton.grenade.Reset();

		skeleton.Activate(Limb.HAND, true);
		skeleton.Activate(Limb.GRENADE, false);
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
		skeleton.Activate(Limb.HAND, true);
    }
}
