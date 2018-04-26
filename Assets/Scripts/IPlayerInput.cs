using UnityEngine;

public interface IPlayerInput {
    // Input functions
    void ApplyInput(PlayerInput input);
    void ApplyInputRelease(PlayerInput inputRelease);
    void ApplyDeltaTime(float deltaTime);

    // Possibly move to separate interface if this gets too messy.
    Vector3 GetPosition();
    void SetPosition(Vector3 position);
}
