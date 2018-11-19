using UnityEngine;
using System.Collections.Generic;

public class LobMotor<T> :
    Motor<T, PhysicsEntity>
	where T : LobMotorData
{
	protected enum Action { NONE, LAUNCH }
	protected Action state;

    private PhysicsInput physicsInput;

    public LobMotor(PhysicsEntity e, Transform t)
		: base(e, t)
	{
        physicsInput = new PhysicsInput();

        FrameCounter.Instance.OnFixedUpdate += HandleFixedUpdate;
	}

    public virtual void HandleFixedUpdate(float deltaTime)
    {
        if (physicsInput.actions.Contains(Action.LAUNCH))
        {
            //var velocity = (data.speed + additiveSpeed) * data.multiplier;
            var velocity = Vector3.zero;

            velocity.x = 400;
            velocity.y = 200;

            entity.SetPosition(root.position);
            entity.SetVelocity(velocity);

            physicsInput.actions.Remove(Action.LAUNCH);
        }
    }
    
    // Handlers end

	public void Lob(CoreDirection lobDirection, Vector3 baseVelocity)
	{
		Debug.AssertFormat(!lobDirection.IsEmpty(), "illegal direction passed");

        state = Action.LAUNCH;
        physicsInput.actions.Add(Action.LAUNCH);
        direction.Update(lobDirection);
    }

	public void Reset()
	{
		state = Action.NONE;
        entity.SetVelocity(Vector3.zero);
        entity.SetPosition(root.position);

        // todo: don't think this should be necessary
        entity.collision.Clear();
	}

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