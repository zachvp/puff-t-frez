using UnityEngine;

public interface IPlayerInput {
    // Input functions
    void ApplyInput(PlayerInputSnapshot input);
    void ApplyDeltaTime(float deltaTime);
}

public interface ITransform {
	Vector3 GetPosition();
    void SetPosition(Vector3 position);
}
