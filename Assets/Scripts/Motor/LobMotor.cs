using UnityEngine;
using System.Collections.Generic;

public class LobMotor<T> :
    Motor<T, PhysicsEntity>, IInputLob
	where T : LobMotorData
{
	private int forceFrameCount;
	private int additiveSpeed; 

	protected enum Action { NONE, LAUNCH, FREEZE }
	protected Action state;

    private PhysicsInput physicsInput;

    public LobMotor(PhysicsEntity e, Transform t)
		: base(e, t)
	{
        physicsInput = new PhysicsInput();

        additiveSpeed = 1;

        FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

    // Handlers begin
	public virtual void HandleUpdate(long currentFrame, float deltaTime)
	{

    }

    public virtual void HandleFixedUpdate(float deltaTime)
    {
        if (physicsInput.actions.Contains(Action.LAUNCH))
        {
            //var velocity = (data.speed + additiveSpeed) * data.multiplier;
            var velocity = direction.Vector * data.speed;

            // Set the velocity direction based on the input direction.
            velocity.x *= direction.Vector.x;

            entity.SetVelocity(velocity);
            physicsInput.actions.Remove(Action.LAUNCH);
        }

        if (physicsInput.actions.Contains(Action.FREEZE))
        {
            Freeze();
        }
    }
    
    // Handlers end

    // ILobmotor begin
	public void Lob(CoreDirection lobDirection, Vector3 baseVelocity)
	{
		Debug.AssertFormat(!lobDirection.IsEmpty(), "illegal direction passed");

		forceFrameCount = data.forceFrameLength;
		state = Action.LAUNCH;

		entity.SetActive(true);
		direction.Update(lobDirection);
        
        // To handle cases when the motor is lobbed from an object in motion,
        // we add the given velocity to our force frames.
		additiveSpeed = Mathf.RoundToInt(Mathf.Abs(baseVelocity.x));
    }
        
	private void Freeze()
	{
        entity.SetVelocity(Vector3.zero);
		state = Action.FREEZE;
        physicsInput.actions.Add(Action.FREEZE);
    }

	public virtual void Reset()
	{
		forceFrameCount = data.forceFrameLength;

		state = Action.NONE;
		entity.SetActive(false);
		entity.SetPosition(root.position);
        entity.collision.Clear();
	}
    // ILobMotor end

    class PhysicsInput
    {
        public HashSet<Action> actions;
        public InputSnapshot<HandGrenadeInput> controlInput;

        public PhysicsInput()
        {
            actions = new HashSet<Action>();
            controlInput = new InputSnapshot<HandGrenadeInput>();
        }
    }
}