﻿using UnityEngine;
using InControl;

public interface ICoreInput<T> where T : CoreInput, IFactoryInput<T>, new()
{
	void ApplyInput(InputSnapshot<T> input);
}

public interface IFactoryInput<T> where T : CoreInput
{
    T Clone();
    T Released(T oldInput);
    T Pressed(T oldInput);
}

public interface IInputPlayerBody
{
	void ApplyInput(InputSnapshot<PlayerInput> input);
    void ApplyDeltaTime(float deltaTime);
}

public interface IInputPlayerHandGrenade
{
	void SetBodyData(MotorData motorData);
	void ApplyInput(InputSnapshot<HandGrenadeInput> input);
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
	void Lob(Vector3 direction, Vector3 baseVelocity);
    void Freeze();
    void Reset();
}

public interface ITransform
{
    void SetPosition(Vector3 position);
	void SetLocalScale(Vector3 scale);
	void SetRotation(Quaternion rotation);
}

// TODO: Consider getting rid of this
public interface IBehavior
{
	void SetActive(bool isActive);
	bool IsActive();
}

public interface IMotor
{
	// TODO: Can maybe get rid of these had have them as Motor read only properties.
    Vector3 GetVelocity();
    Vector3 GetDirection();
}
