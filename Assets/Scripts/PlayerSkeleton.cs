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

	public PlayerMotor body;

	private Limb existing;
    private Limb active;


	public PlayerSkeleton(PlayerMotor playerMotor,
	                      IdleLimbMotor handMotor,
	                      IdleLimbMotor footMotor,
	                      PlayerGrenadeMotor grenadeMotor)
	{
		AttachBody(playerMotor)
		.AttachHand(handMotor)
		.AttachFoot(footMotor)
		.AttachGrenade(grenadeMotor);
	}

	public PlayerSkeleton AttachBody(PlayerMotor motor)
	{
		body = motor;
		body.entity.OnActivationChange += HandleBodyActivationChange;

		AttachLimb(Limb.BODY, body.entity);

		return this;
	}

	public PlayerSkeleton AttachHand(IdleLimbMotor motor)
	{
		hand = motor;
        hand.entity.OnActivationChange += HandleHandActivationChange;

		AttachLimb(Limb.HAND, hand.entity);

		return this;
	}

	public PlayerSkeleton AttachFoot(IdleLimbMotor motor)
    {
        foot = motor;
        foot.entity.OnActivationChange += HandleFootActivationChange;

		AttachLimb(Limb.FOOT, foot.entity);

        return this;
    }

	public PlayerSkeleton AttachGrenade(PlayerGrenadeMotor motor)
    {
        grenade = motor;
        grenade.entity.OnActivationChange += HandleGrenadeActivationChange;

		AttachLimb(Limb.GRENADE, grenade.entity);

        return this;
    }

	// Handlers
    public void HandleBodyActivationChange(bool isActive)
    {
        HandleLimbActivationChange(isActive, Limb.BODY);
    }

    public void HandleHandActivationChange(bool isActive)
    {
        HandleLimbActivationChange(isActive, Limb.HAND);
    }

    public void HandleGrenadeActivationChange(bool isActive)
    {
        HandleLimbActivationChange(isActive, Limb.GRENADE);
    }

    public void HandleFootActivationChange(bool isActive)
    {
        HandleLimbActivationChange(isActive, Limb.FOOT);
    }

    // Private
	private PlayerSkeleton AttachLimb(Limb limb, Entity entity)
	{
		FlagsHelper.Set(ref existing, limb);
		Activate(limb, true);
		entity.SetAffinity(Affinity.PLAYER);
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

    public void Activate(Limb limbs, bool isActive)
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
    }

    public bool IsActive(Limb limbs)
    {
        var result = false;

        if (FlagsHelper.IsSet(limbs, Limb.BODY))
        {
            result |= FlagsHelper.IsSet(active, Limb.BODY);
        }
        if (FlagsHelper.IsSet(limbs, Limb.FOOT))
        {
            result |= FlagsHelper.IsSet(active, Limb.FOOT);
        }
        if (FlagsHelper.IsSet(limbs, Limb.HAND))
        {
            result |= FlagsHelper.IsSet(active, Limb.HAND);
        }
        if (FlagsHelper.IsSet(limbs, Limb.GRENADE))
        {
            result |= FlagsHelper.IsSet(active, Limb.GRENADE);
        }

        return result;
    }
}

[Flags]
public enum Limb
{
    NONE = 0,
    BODY = 1 << 0,
    HAND = 1 << 1,
    FOOT = 1 << 2,
    GRENADE = 1 << 3
}
