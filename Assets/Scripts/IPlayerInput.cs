using UnityEngine;

public interface IPlayerInput {
    // Input functions
    void ApplyInput(PlayerInputSnapshot input);
    void ApplyDeltaTime(float deltaTime);
}

public interface ITransform {
    void SetPosition(Vector3 position);
	void SetLocalScale(Vector3 scale);
}
