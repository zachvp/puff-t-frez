using System;

public class InputController<T> where T : IFactoryInput<T>, new()
{
	protected T input;
	protected T inputRelease;
	protected T lastInput;

	protected InputBuffer<InputSnapshot<T>> buffer;

	public InputController()
	{
		input = new T();

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public virtual void HandleUpdate(int currentFrame, float deltaTime)
    {
		// Get the data ready for the new frame
		inputRelease = new T();
		lastInput = input.Clone();
		input = new T();
    }
}
