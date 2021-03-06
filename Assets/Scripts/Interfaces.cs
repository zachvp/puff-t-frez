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

public interface IInputLob
{
	void Lob(CoreDirection direction, Vector3 baseVelocity);
    void Reset();
}

public interface ITransform
{
    void SetPosition(Vector3 position);
	void SetLocalScale(Vector3 scale);
	void SetRotation(Quaternion rotation);
    void SetParent(Transform parent);
}

// TODO: Consider getting rid of this
public interface IBehavior
{
	void SetActive(bool isActive);
	bool IsActive();
}
