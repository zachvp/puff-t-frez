// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
public class PlayerMarionette :
      ICoreInput<HandGrenadeInput>,
      ICoreInput<PlayerInput>
{
	private PlayerSkeleton skeleton;
    
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
		skeleton.body.ApplyInput(input);
	}

	public void ApplyInput(InputSnapshot<HandGrenadeInput> input)
	{
		if (input.pressed.launch && !skeleton.IsActive(Limb.GRENADE))
		{
			var bodyData = new MotorData(skeleton.body.GetDirection(),
			                             skeleton.body.GetVelocity());

			input.held.data = bodyData;

			UnityEngine.Debug.LogFormat("Input dir flags: {0}", input.held.direction.flags);
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
		if (FlagsHelper.IsSet(skeleton, Limb.HAND | Limb.BODY, LogicMode.AND))
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
