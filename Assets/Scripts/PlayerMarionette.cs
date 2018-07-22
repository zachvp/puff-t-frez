using System;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
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

		FlagsHelper.Set(ref skeleton, Skeleton.HAND);
		Activate(Skeleton.HAND, true);
		CheckReset();
	}

	public void AttachHandGrenade(PlayerHandGrenadeMotor motor)
	{
		// TODO: Possibly set hand modifications mask?
		handGrenadeMotor = motor;
		handGrenadeMotor.OnPickup += HandleGrenadePickup;

		FlagsHelper.Set(ref skeleton, Skeleton.GRENADE);
		Activate(Skeleton.GRENADE, false);
		//handGrenadeMotor.OnReset += Reset;
	}

	// IPlayerMarionette begin
	public void ApplyPlayerInput(InputSnapshot<PlayerInput> input)
	{
		bodyMotor.ApplyInput(input);
	}

	public void ApplyGrenadeInput(InputSnapshot<HandGrenadeInput> input)
	{
		var bodyDirection = CoreUtilities.Convert(bodyMotor.GetDirection());
		var bodyData = new MotorData(bodyDirection, bodyMotor.GetVelocity());

		handGrenadeMotor.SetBodyData(bodyData);
		handGrenadeMotor.ApplyInput(input);

		if (input.pressed.launch && IsActive(Skeleton.HAND))
		{
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
			FlagsHelper.Set(ref active, Skeleton.BODY);
			bodyMotor.entity.SetActive(isActive);
		}
		if (FlagsHelper.IsSet(limbs, Skeleton.FOOT))
		{
			FlagsHelper.Set(ref active, Skeleton.FOOT);
			// TODO: imp
		}
		if (FlagsHelper.IsSet(limbs, Skeleton.HAND))
		{
			FlagsHelper.Set(ref active, Skeleton.HAND);
			handEntity.SetActive(isActive);
		}
		if (FlagsHelper.IsSet(limbs, Skeleton.GRENADE))
		{
			FlagsHelper.Set(ref active, Skeleton.GRENADE);
			handGrenadeMotor.entity.SetActive(isActive);
		}
	}

	private bool IsActive(Skeleton limbs)
	{
		var result = false;

		result |= FlagsHelper.IsSet(limbs, Skeleton.BODY);
		result |= FlagsHelper.IsSet(limbs, Skeleton.FOOT);
		result |= FlagsHelper.IsSet(limbs, Skeleton.HAND);
		result |= FlagsHelper.IsSet(limbs, Skeleton.GRENADE);

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
