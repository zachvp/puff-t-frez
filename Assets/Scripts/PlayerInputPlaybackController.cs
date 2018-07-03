using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Records and plays back player input
[RequireComponent(
        typeof(PlayerMotor),
        typeof(PlayerCharacterInitializer)
    )
]
public class PlayerInputPlaybackController : IPlayerInputPlaybackInformer {
	public static EventHandler OnPlaybackStarted;
	public static EventHandler<IPlayerInputPlaybackInformer> OnInformInitialData;

	private PlayerMotor player;

    private InputBuffer buffer;
    private Vector3 initialPosition;
    
#if DEBUG
    private Vector3 finalPosition;
#endif
    
	// TODO: Keep map of played-back Transforms. At the end, iterate and assert.
	public PlayerInputPlaybackController(PlayerMotor playerMotor, InputBuffer inputBuffer) {
		player = playerMotor;
		buffer = inputBuffer;
	}

	public void Start() {
        initialPosition = player.GetPosition();

		Events.Raise(OnInformInitialData, this);
	}

	public void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
			Events.Raise(OnPlaybackStarted);

            finalPosition = player.GetPosition();

            player.SetPosition(initialPosition);
			CoreBehaviour.Instance.StartCoroutine(PlaybackFrames());
        }
	}

    private IEnumerator PlaybackFrames() {
        var bufferCopy = new List<PlayerInputSnapshot>(buffer.inputBuffer);
        var timeCopy = new List<float>(FrameCounter.Instance.deltaTimes);
        var i = 0;

        Debug.AssertFormat(bufferCopy.Count == timeCopy.Count, "Input buffer length {0} different from DeltaTime length {1}", bufferCopy.Count, timeCopy.Count);

        foreach (PlayerInputSnapshot input in bufferCopy) {
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

	// IPlayerInputPlaybackInformer
	public void InformInitialTransform(Transform t) {
		// TODO: waaat is this ^ ?
	}

	class PlaybackObject {
		public StoreTransform initialTransform;

		public PlaybackObject(Transform t) {
			initialTransform = new StoreTransform(t);
		}
	}
}

public interface IPlayerInputPlaybackInformer {
	void InformInitialTransform(Transform t);
}
