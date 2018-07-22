using System;

public class InputController<T, U> where T : IFactoryInput<T>, new() where U : class
{
	
	protected T oldInput;
	protected T input;
	protected U responder;

	protected InputBuffer<InputSnapshot<T>> buffer;

	public InputController(U r)
	{
		input = new T();
		responder = r;

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public virtual void HandleUpdate(long currentFrame, float deltaTime)
    {
		// Get the data ready for the new frame
		oldInput = input.Clone();
		input = new T();
    }
}
