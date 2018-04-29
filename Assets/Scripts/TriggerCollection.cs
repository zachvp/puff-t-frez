using UnityEngine;

public class TriggerCollection : MonoBehaviour {
    public Trigger below;

    public CharacterCollisionState2D collision { get; private set; }

	public void Awake() {
        collision = new CharacterCollisionState2D();

        Debug.AssertFormat(below != null, "Trigger below is null!");

        below.OnEnter += HandleEnterBelow;
        below.OnExit += HandleExitBelow;
	}

    private void HandleEnterBelow() {
        Debug.LogFormat("Handle enter below");
        collision.below = true;
    }

    private void HandleExitBelow() {
        Debug.LogFormat("Handle exit below");
        collision.below = false;
    }
}
