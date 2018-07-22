using System;
using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
using System.Diagnostics;
public class PlayerMarionette : IPlayerMarionette
{
	PlayerMotor bodyMotor;
        
	private IBehavior handBehavior;
	private Entity handEntity; 
    
	PlayerHandGrenadeMotor handGrenadeMotor;
    
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
		bodyMotor = motor;

		FlagsHelper.Set(ref skeleton, Skeleton.BODY);
		Activate(Skeleton.BODY, true);
		CheckReset();
	}

	public void AttachHand(IBehavior behavior, Entity entity)
	{
		handBehavior = behavior;
		handEntity = entity;

		handEntity.OnActivationChange += HandleHandActivationChange;

		FlagsHelper.Set(ref skeleton, Skeleton.HAND);
		Activate(Skeleton.HAND, true);
		CheckReset();
	}

    public void HandleHandActivationChange(bool isActive)
	{
		HandleLimbActivationChange(isActive, Skeleton.HAND);
	}

	public void HandleGrenadeActivationChange(bool isActive)
	{
		HandleLimbActivationChange(isActive, Skeleton.GRENADE);
	}

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

	public void AttachHandGrenade(PlayerHandGrenadeMotor motor)
	{
		handGrenadeMotor = motor;
		handGrenadeMotor.OnPickup += HandleGrenadePickup;

		handGrenadeMotor.entity.OnActivationChange += HandleGrenadeActivationChange;

		FlagsHelper.Set(ref skeleton, Skeleton.GRENADE);
		Activate(Skeleton.GRENADE, false);
	}

	// IPlayerMarionette begin
	public void ApplyPlayerInput(InputSnapshot<PlayerInput> input)
	{
		bodyMotor.ApplyInput(input);
	}

	public void ApplyGrenadeInput(InputSnapshot<HandGrenadeInput> input)
	{        
		if (input.pressed.launch && !IsActive(Skeleton.GRENADE))
		{
			var bodyDirection = CoreUtilities.Convert(bodyMotor.GetDirection());
            var bodyData = new MotorData(bodyDirection, bodyMotor.GetVelocity());

			handGrenadeMotor.SetBodyData(bodyData);
            handGrenadeMotor.ApplyInput(input);

			Activate(Skeleton.GRENADE, true);
			Activate(Skeleton.HAND, false);
            handGrenadeMotor.entity.SetPosition(handEntity.Position);
		}
	}

	public void ApplyDeltaTime(float deltaTime)
	{
		bodyMotor.ApplyDeltaTime(deltaTime);
		handGrenadeMotor.ApplyDeltaTime(deltaTime);
	}
	// IPlayerMarionette end

    // Handlers
    public void HandleGrenadePickup()
	{
		handEntity.SetPosition(handGrenadeMotor.entity.Position);
		handGrenadeMotor.Reset();

		Activate(Skeleton.HAND, true);
        Activate(Skeleton.GRENADE, false);
	}

    // Private
	private void Reset()
    {
        handEntity.SetPosition(bodyMotor.entity.Position);
        Activate(Skeleton.HAND, true);
    }

	private void Activate(Skeleton limbs, bool isActive)
	{
		if (FlagsHelper.IsSet(limbs, Skeleton.BODY))
		{
			bodyMotor.entity.SetActive(isActive);
		}
		if (FlagsHelper.IsSet(limbs, Skeleton.FOOT))
		{
			UnityEngine.Debug.LogWarning("Unimplemented functionality");
			// TODO: imp
		}
		if (FlagsHelper.IsSet(limbs, Skeleton.HAND))
		{
			handEntity.SetActive(isActive);
		}
		if (FlagsHelper.IsSet(limbs, Skeleton.GRENADE))
		{
			handGrenadeMotor.entity.SetActive(isActive);
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
