using UnityEngine;

public class PlayerCharacterEntity : Entity {
	// Anchors
	public Transform handAnchor;
	public Transform footAnchor;

	// Limb references
	public Entity handTemplate;
	public Entity footTemplate;
}
