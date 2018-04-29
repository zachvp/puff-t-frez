using UnityEngine;

public class Trigger : MonoBehaviour {
    public EventHandler OnEnter;
    public EventHandler OnExit;

	void OnTriggerEnter2D(Collider2D other) {
        Events.Raise(OnEnter);
	}

    void OnTriggerExit2D(Collider2D other) {
        Events.Raise(OnExit);
	}
}
