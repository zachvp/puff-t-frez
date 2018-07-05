using UnityEngine;

public interface ILobInput {
    // TODO: Add params?
	void Lob(Direction2D direction, Vector3 baseVelocity);
    void Freeze();
	void Reset();
}
