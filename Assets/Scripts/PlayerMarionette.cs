using System;
using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
public class PlayerMarionette : IPlayerMarionette
{
	private PlayerMotor body;
	private IdleLimbMotor hand;
	private PlayerHandGrenadeMotor grenade;
    
	[Flags]
    public enum Skeleton
	{
		NONE    = 0,
		BODY    = 1 << 0,
        HAND    = 1 << 1,
        FOOT    = 1 << 2,
        GRENADE = 1 << 3
	}
	private Skeleton skeleton;
	private Skeleton active;
    
	public void AttachBody(PlayerMotor motor)
	{
		body = motor;

		FlagsHelper.Set(ref skeleton, Skeleton.BODY);
		Activate(Skeleton.BODY, true);
		CheckReset();
	}

	public void AttachHand(IdleLimbMotor motor)
	{
		hand = motor;

		hand.entity.OnActivationChange += HandleHandActivationChange;

		FlagsHelper.Set(ref skeleton, Skeleton.HAND);
		Activate(Skeleton.HAND, true);
		CheckReset();
	}

	public void AttachGrenade(PlayerHandGrenadeMotor motor)
	{
		grenade = motor;
		grenade.OnPickup += HandleGrenadePickup;

		grenade.entity.OnActivationChange += HandleGrenadeActivationChange;

		FlagsHelper.Set(ref skeleton, Skeleton.GRENADE);
		Activate(Skeleton.GRENADE, false);
	}

	// IPlayerMarionette begin
	public void ApplyPlayerInput(InputSnapshot<PlayerInput> input)
	{
		body.ApplyInput(input);
	}

	public void ApplyGrenadeInput(InputSnapshot<HandGrenadeInput> input)
	{        
		if (input.pressed.launch && !IsActive(Skeleton.GRENADE))
		{
			var bodyDirection = CoreUtilities.Convert(body.GetDirection());
            var bodyData = new MotorData(bodyDirection, body.GetVelocity());

			grenade.SetBodyData(bodyData);
            grenade.ApplyInput(input);

			Activate(Skeleton.GRENADE, true);
			Activate(Skeleton.HAND, false);
			grenade.entity.SetPosition(hand.entity.Position);
		}
	}

	public void ApplyDeltaTime(float deltaTime)
	{
		body.ApplyDeltaTime(deltaTime);
		grenade.ApplyDeltaTime(deltaTime);
	}
	// IPlayerMarionette end

    // Handlers
	public void HandleHandActivationChange(bool isActive)
    {
        HandleLimbActivationChange(isActive, Skeleton.HAND);
    }

    public void HandleGrenadeActivationChange(bool isActive)
    {
        HandleLimbActivationChange(isActive, Skeleton.GRENADE);
    }

    public void HandleGrenadePickup()
	{
		hand.entity.SetPosition(grenade.entity.Position);
		grenade.Reset();

		Activate(Skeleton.HAND, true);
        Activate(Skeleton.GRENADE, false);
	}

    // Private
	private void HandleLimbActivationChange(bool isActive, Skeleton limb)
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

	private void Reset()
    {
		hand.entity.SetPosition(body.entity.Position);
        Activate(Skeleton.HAND, true);
    }

	private void Activate(Skeleton limbs, bool isActive)
	{
		if (FlagsHelper.IsSet(limbs, Skeleton.BODY))
		{
			body.entity.SetActive(isActive);
		}
		if (FlagsHelper.IsSet(limbs, Skeleton.FOOT))
		{
			UnityEngine.Debug.LogWarning("Unimplemented functionality");
			// TODO: imp
		}
		if (FlagsHelper.IsSet(limbs, Skeleton.HAND))
		{
			hand.entity.SetActive(isActive);
		}
		if (FlagsHelper.IsSet(limbs, Skeleton.GRENADE))
		{
			grenade.entity.SetActive(isActive);
		}
	}

	private bool IsActive(Skeleton limbs)
	{
		var result = false;

		if (FlagsHelper.IsSet(limbs, Skeleton.BODY))
        {
			result |= FlagsHelper.IsSet(active, Skeleton.BODY);
        }
        if (FlagsHelper.IsSet(limbs, Skeleton.FOOT))
        {
			result |= FlagsHelper.IsSet(active, Skeleton.FOOT);
        }
        if (FlagsHelper.IsSet(limbs, Skeleton.HAND))
        {
			result |= FlagsHelper.IsSet(active, Skeleton.HAND);
        }
        if (FlagsHelper.IsSet(limbs, Skeleton.GRENADE))
        {
            result |= FlagsHelper.IsSet(active, Skeleton.GRENADE);
        }

		return result;
	}

    private void CheckReset()
	{
		if (FlagsHelper.IsSet(skeleton, Skeleton.HAND) &&
		    FlagsHelper.IsSet(skeleton, Skeleton.BODY))
		{
			Reset();
		}
	}
}
