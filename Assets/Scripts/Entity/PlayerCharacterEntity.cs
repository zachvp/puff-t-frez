using UnityEngine;

public class PlayerCharacterEntity : PhysicsEntity
{
	// Anchors
	public Transform handAnchorRight;
	public Transform handAnchorLeft;

	public Transform footAnchorRight;
	public Transform footAnchorLeft;

    new public BoxCollider2D collider;

    public override void Awake()
    {
        base.Awake();

        collider = GetComponent<BoxCollider2D>();
    }
}
