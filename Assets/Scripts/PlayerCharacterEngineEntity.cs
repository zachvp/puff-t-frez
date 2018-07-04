using UnityEngine;

public class PlayerCharacterEngineEntity : EngineEntity {
	// Anchors
	public Transform handAnchor;
	public Transform footAnchor;

	// Limb references
	public EngineEntity hand;
	public EngineEntity foot;
}
