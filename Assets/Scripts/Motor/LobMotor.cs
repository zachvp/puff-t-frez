using UnityEngine;
using System.Collections.Generic;

public class LobMotor<T> :
    Motor<T, PhysicsEntity>, IInputLob
	where T : LobMotorData
{
	private int forceFrameCount;
	private int additiveSpeed; 

	protected enum State { NONE, LAUNCHED, FREEZE }
	protected State state;

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
        if (entity.collision.current.IsColliding(Constants.Layers.OBSTACLE))
        {
            physicsInput.states.Add(State.FREEZE);
        }

        if (state == State.NONE)
		{
			entity.SetPosition(root.position);
		}
		else if (state == State.LAUNCHED)
		{
            physicsInput.states.Add(State.LAUNCHED);
        }
    }

    public virtual void HandleFixedUpdate(float deltaTime)
    {
        if (physicsInput.states.Contains(State.LAUNCHED))
        {
            var velocity = Vector3.zero;

            // Determine if force should be applied 
            if (forceFrameCount > 0)
            {
                var multiplier = 1 - (forceFrameCount / data.forceFrameLength);
                var speed = data.speed + additiveSpeed;

                velocity = speed * data.multiplier * multiplier;

                // Set the velocity direction based on the input direction.
                velocity.x *= direction.Vector.x;
                --forceFrameCount;
            }

            entity.SetVelocity(velocity);
            physicsInput.states.Remove(State.LAUNCHED);
        }

        if (physicsInput.states.Contains(State.FREEZE))
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
		state = State.LAUNCHED;

		entity.SetActive(true);
		direction.Update(lobDirection);
        
        // To handle cases when the motor is lobbed from an object in motion,
        // we add the given velocity to our force frames.
		additiveSpeed = Mathf.RoundToInt(Mathf.Abs(baseVelocity.x));
    }
        
	private void Freeze()
	{
        entity.SetVelocity(Vector3.zero);
		state = State.FREEZE;
        physicsInput.states.Add(State.FREEZE);
    }

	public virtual void Reset()
	{
		forceFrameCount = data.forceFrameLength;

		state = State.NONE;
		entity.SetActive(false);
		entity.SetPosition(root.position);
        entity.collision.Clear();
	}
    // ILobMotor end

    class PhysicsInput
    {
        public HashSet<State> states;
        public InputSnapshot<HandGrenadeInput> controlInput;

        public PhysicsInput()
        {
            states = new HashSet<State>();
            controlInput = new InputSnapshot<HandGrenadeInput>();
        }
    }
}