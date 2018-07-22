// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
public class PlayerMarionette : IPlayerMarionette
{
	private PlayerSkeleton skeleton;
        
	public PlayerMarionette(PlayerSkeleton playerSkeleton)
	{
		skeleton = playerSkeleton;
		skeleton.OnLimbAttached += HandleLimbAttached;

		skeleton.grenade.OnGrab += HandleGrenadePickup;
		skeleton.Activate(Limb.GRENADE, false);
	}

	// IPlayerMarionette begin
	public void ApplyPlayerInput(InputSnapshot<PlayerInput> input)
	{
		skeleton.body.ApplyInput(input);
	}

	public void ApplyGrenadeInput(InputSnapshot<HandGrenadeInput> input)
	{        
		if (input.pressed.launch && !skeleton.IsActive(Limb.GRENADE))
		{
			var bodyDirection = CoreUtilities.Convert(skeleton.body.GetDirection());
			var bodyData = new MotorData(bodyDirection, skeleton.body.GetVelocity());

			skeleton.grenade.SetBodyData(bodyData);
			skeleton.grenade.ApplyInput(input);

			skeleton.Activate(Limb.GRENADE, true);
			skeleton.Activate(Limb.HAND, false);
			skeleton.grenade.entity.SetPosition(skeleton.hand.entity.Position);
		}
	}

	public void ApplyDeltaTime(float deltaTime)
	{
		skeleton.body.ApplyDeltaTime(deltaTime);
		skeleton.grenade.ApplyDeltaTime(deltaTime);
	}
	// IPlayerMarionette end

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
		if (FlagsHelper.IsSet(skeleton, Limb.HAND) &&
		    FlagsHelper.IsSet(skeleton, Limb.BODY))
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
