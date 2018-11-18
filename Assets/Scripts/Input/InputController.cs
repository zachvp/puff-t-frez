using UnityEngine;

public class InputController<T, U>
	where T : CoreInput, IFactoryInput<T>, new()
	where U : ICoreInput<T>, new()
{
	protected T oldInput;
	protected T input;
	protected U responder;

	protected InputSnapshot<T> snapshot;
	protected InputBuffer<InputSnapshot<T>> buffer;

	public InputController(ICoreInput<T> r, InputBuffer<InputSnapshot<T>> b)
	{
		input = new T();
		responder = (U) r;
		buffer = b;

		FrameCounter.Instance.OnUpdate += HandleUpdate;
    	FrameCounter.Instance.OnLateUpdate += HandleLateUpdate;
	}

    // todo: make not virtual
	public void HandleUpdate(long currentFrame, float deltaTime)
    {
		// Get the data ready for the new frame
		oldInput = input.Clone();
		input = new T();

        UpdateInput();

        Debug.AssertFormat(CoreUtilities.IsConstrained(input.direction.Vector, 1),
                   "invalid input");

        // Check if input directions should be neutralized;
        input.direction.ClearConcurrent();
        snapshot = new InputSnapshot<T>(oldInput, input);
        buffer.AddInput(snapshot);
        responder.ApplyInput(snapshot);
    }

    // todo: move to interface and have reference to that interface instead?
    protected virtual void UpdateInput()
    {
        Debug.Assert(false, "Child class must implement this.");
    }

	public virtual void HandleLateUpdate()
	{

	}

	public Vector2 RemoveDeadZone(Vector2 v)
	{
		var r = Vector2.zero;

		r.x = Mathf.Abs(v.x) > 0.1f ? v.x : r.x;
		r.y = Mathf.Abs(v.y) > 0.1f ? v.y : r.y;

		return r;
	}
}
