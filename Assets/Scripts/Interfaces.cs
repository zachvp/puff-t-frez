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
	// TODO: Should be Direction2D enum mask
    Vector3 GetDirection();
}

public interface IFactoryInput<T>
{
	T Clone();
	T Released(T oldInput);
	T Pressed(T oldInput);
}
