﻿public class InputController<T, U>
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

	public virtual void HandleUpdate(long currentFrame, float deltaTime)
    {
		// Get the data ready for the new frame
		oldInput = input.Clone();
		input = new T();
    }

	public virtual void HandleLateUpdate()
	{
		// Check if input directions should be neutralized;
        input.ClearConcurrent();
        snapshot = new InputSnapshot<T>(oldInput, input);
        buffer.AddInput(snapshot);
		responder.ApplyInput(snapshot);
	}
}
