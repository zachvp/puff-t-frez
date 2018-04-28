using UnityEngine;

public class Trigger : MonoBehaviour {
	void OnTriggerEnter2D(Collider2D other) {
        Debug.LogFormat("trigger enter: {0}", other.name);
	}

    void OnTriggerExit2D(Collider2D other) {
        Debug.LogFormat("trigger exit: {0}", other.name);
	}
}
