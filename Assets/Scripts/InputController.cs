using System;

public class InputController<T> where T : IFactoryInput<T>, new()
{
	protected T oldInput;
	protected T input;

	protected InputBuffer<InputSnapshot<T>> buffer;

	public InputController()
	{
		input = new T();

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public virtual void HandleUpdate(long currentFrame, float deltaTime)
    {
		// Get the data ready for the new frame
		oldInput = input.Clone();
		input = new T();
    }
}
