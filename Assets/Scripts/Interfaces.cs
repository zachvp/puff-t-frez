using UnityEngine;

public interface IInputPlayerBody
{
    // Input functions
    void ApplyInput(PlayerInputSnapshot input);
    void ApplyDeltaTime(float deltaTime);
}

public interface IPlayerMarionette
{
    void ApplyPlayerInput(PlayerInputSnapshot snapshot);
    void ApplyGrenadeInput();

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
