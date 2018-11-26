using System;
using UnityEngine;

public class PlayerSkeleton
{
	// Param 0: Full skeleton
	// Param 1: Newly attached limb
	public EventHandler<Limb, Limb> OnLimbAttached;

	public IdleLimbMotor hand { get; private set; }
	public IdleLimbMotor foot { get; private set; }
	public PlayerGrenadeMotor grenade { get; private set; }
	public PlayerMotor body { get; private set; }
	public PlayerHandMotor combatHand { get; private set; }

	private Limb existing;
    private Limb active;

	public PlayerSkeleton AttachBody(PlayerMotor motor)
	{
		body = motor;
		body.entity.OnActivationChange += delegate(bool isActive)
		{
			HandleLimbActivationChange(isActive, Limb.BODY);
		};

		AttachLimb(Limb.BODY);

		return this;
	}

	public PlayerSkeleton AttachHand(IdleLimbMotor motor)
	{
		hand = motor;
        hand.entity.OnActivationChange += delegate(bool isActive)
		{
			HandleLimbActivationChange(isActive, Limb.HAND);
		};

		AttachLimb(Limb.HAND);

		return this;
	}

	public PlayerSkeleton AttachFoot(IdleLimbMotor motor)
    {
        foot = motor;
        foot.entity.OnActivationChange += delegate(bool isActive)
		{
			HandleLimbActivationChange(isActive, Limb.FOOT);
		};
       
		AttachLimb(Limb.FOOT);

        return this;
    }

	public PlayerSkeleton AttachGrenade(PlayerGrenadeMotor motor)
    {
        grenade = motor;
        grenade.entity.OnActivationChange += delegate(bool isActive)
		{
			HandleLimbActivationChange(isActive, Limb.GRENADE);
		};

		AttachLimb(Limb.GRENADE);

        return this;
    }

	public PlayerSkeleton AttachCombatHand(PlayerHandMotor motor)
	{
		combatHand = motor;
		// todo: use anonymous delegates
		combatHand.entity.OnActivationChange += delegate(bool isActive)
		{
			HandleLimbActivationChange(isActive, Limb.COMBAT_HAND);
		};

		AttachLimb(Limb.COMBAT_HAND);

		return this;
	}

    // Private
	private PlayerSkeleton AttachLimb(Limb limb)
	{
		FlagsHelper.Set(ref existing, limb);
		SetActive(limb, true);
		Events.Raise(OnLimbAttached, existing, limb);

		return this;
	}

	private void HandleLimbActivationChange(bool isActive, Limb limb)
    {
        if (isActive)
        {
            FlagsHelper.Set(ref active, limb);
        }
        else
        {
            FlagsHelper.Unset(ref active, limb);
        }
    }

	// todo: optimize so that new case isn't needed for every new limb
    public void SetActive(Limb limbs, bool isActive)
    {
        if (FlagsHelper.IsSet(limbs, Limb.BODY))
        {
            body.entity.SetActive(isActive);
        }
        if (FlagsHelper.IsSet(limbs, Limb.FOOT))
        {
            foot.entity.SetActive(isActive);
        }
        if (FlagsHelper.IsSet(limbs, Limb.HAND))
        {
            hand.entity.SetActive(isActive);
        }
        if (FlagsHelper.IsSet(limbs, Limb.GRENADE))
        {
            grenade.entity.SetActive(isActive);
        }
		if (FlagsHelper.IsSet(limbs, Limb.COMBAT_HAND))
		{
			combatHand.entity.SetActive(isActive);
		}
    }

	// todo: this seems suboptimal
	public bool IsActive(Limb limbs, Logical mode)
    {
		return FlagsHelper.IsSet(active, limbs, mode);
    }
}

[Flags]
public enum Limb
{
    NONE = 0,
    BODY = 1 << 0,
    HAND = 1 << 1,
    FOOT = 1 << 2,
    GRENADE = 1 << 3,
    COMBAT_HAND = 1 << 4
}
