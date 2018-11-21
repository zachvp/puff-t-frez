using UnityEngine;

public class PlayerCharacterEntity : PhysicsEntity
{
	// Anchors
	public Transform handAnchorRight;
	public Transform handAnchorLeft;

	public Transform footAnchorRight;
	public Transform footAnchorLeft;

    public override void Awake()
    {
        base.Awake();
    }
}
