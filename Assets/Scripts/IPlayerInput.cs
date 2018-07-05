using UnityEngine;

// TODO: Refactor to reference player body
public interface IPlayerInput
{
    // Input functions
    void ApplyInput(PlayerInputSnapshot input);
    void ApplyDeltaTime(float deltaTime);
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
