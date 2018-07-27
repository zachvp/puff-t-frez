using UnityEngine;

public class PlayerCharacterEntity : Entity
{
	// Anchors
	public Transform handAnchorRight;
	public Transform handAnchorLeft;

	public Transform footAnchorRight;
	public Transform footAnchorLeft;
    
	new public BoxCollider2D Collider
    {
		get { return (BoxCollider2D) collider; }
    }
}
