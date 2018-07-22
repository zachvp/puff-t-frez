using UnityEngine;

public class PlayerCharacterEntity : Entity
{
	// Anchors
	public Transform handAnchor;
	public Transform footAnchor;

	new public BoxCollider2D Collider
    {
		get { return (BoxCollider2D) collider; }
    }
}
