using UnityEngine;
using InControl;

public interface IInputPlayerBody
{
    // Input functions
	void ApplyInput(InputSnapshot<PlayerInput> input);
    void ApplyDeltaTime(float deltaTime);
}

public interface IPlayerMarionette
{
	void ApplyPlayerInput(InputSnapshot<PlayerInput> snapshot);
	void ApplyGrenadeInput(InputSnapshot<HandGrenadeInput> snapshot);

    void ApplyDeltaTime(float deltaTime);
}

public interface IInputLob
{
    void Lob(Direction2D direction, Vector3 baseVelocity);
    void Freeze();
    void Reset();
}

public interface ITransform
{
    void SetPosition(Vector3 position);
	void SetLocalScale(Vector3 scale);
	void SetRotation(Quaternion rotation);
}

public interface IBehavior
{
	void SetActive(bool isActive);
}

public interface IMotor
{
    Vector3 GetVelocity();
    Vector3 GetDirection();
}

public interface IFactory<T>
{
	T Clone();
}

// TODO: This should be in a different class
public class CoreObject : System.Object
{
	public CoreObject()
	{
		
	}

	public CoreObject(CoreObject other)
	{
		Debug.Assert(false, "TODO: This method must be implemented");
	}
}
