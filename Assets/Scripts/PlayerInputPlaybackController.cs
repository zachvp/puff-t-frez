using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Records and plays back player input
[RequireComponent(
        typeof(PlayerMotor),
        typeof(PlayerCharacterInitializer)
    )
]
public class PlayerInputPlaybackController : MonoBehaviour {
    private IPlayerInput player;
    private InputBuffer buffer;
    private Vector3 initialPosition;

    // TODO: DBG var
    private Vector3 finalPosition;

    public void Awake() {
        var initializer = GetComponent<PlayerCharacterInitializer>();

        player = GetComponent<PlayerMotor>();

        initializer.OnCreate += HandleCreate;
    }

	public void Start() {
        initialPosition = player.GetPosition();
	}

	public void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            finalPosition = player.GetPosition();

            player.SetPosition(initialPosition);
            StartCoroutine(PlaybackFrames());
        }
	}

    private IEnumerator PlaybackFrames() {
        var bufferCopy = new List<Vector2>(buffer.inputBuffer);
        var timeCopy = new List<float>(FrameCounter.Instance.deltaTimes);
        var i = 0;

        Debug.AssertFormat(bufferCopy.Count == timeCopy.Count, "Input buffer length {0} different from DeltaTime length {1}", bufferCopy.Count, timeCopy.Count);

        foreach (Vector2 input in bufferCopy) {
            var deltaTime = timeCopy[i];

            player.ApplyInput(input);
            player.ApplyDeltaTime(deltaTime);
            i++;
            yield return null;
        }

        Debug.AssertFormat(player.GetPosition() == finalPosition, "Played back final position {0} did not match actual final position {1}", player.GetPosition(), finalPosition);
        Debug.LogFormat("Finished playback");

        yield break;
    }

	private void HandleCreate(PlayerCharacterInitializer initializer) {
        buffer = initializer.inputBuffer;
    }
}
