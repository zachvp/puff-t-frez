using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Records and plays back player input
public class InputPlaybackControllerPlayer
{
	public static EventHandler OnPlaybackStarted;

	private IInputPlayerBody player;
	private Entity entity;

	private InputBuffer<PlayerInputSnapshot> buffer;
    private Vector3 initialPosition;
    
#if DEBUG
    private Vector3 finalPosition;
#endif
    
	public InputPlaybackControllerPlayer(IInputPlayerBody playerMotor,
	                                     Entity playerTransform,
	                                     InputBuffer<PlayerInputSnapshot> inputBuffer)
	{
		player = playerMotor;
		entity = playerTransform;
		buffer = inputBuffer;

		initialPosition = entity.position;

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public void HandleUpdate(int currentFrame, float deltaTime)
	{
        if (Input.GetKeyDown(KeyCode.R)) {
			Events.Raise(OnPlaybackStarted);

			finalPosition = entity.position;

			entity.SetPosition(initialPosition);
			CoreBehaviour.Instance.StartCoroutine(PlaybackFrames());
        }
	}

	// TODO: This is slightly off. Think it has to do with the frame buffer count mismatch.
    private IEnumerator PlaybackFrames()
	{
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

		Debug.AssertFormat(entity.position == finalPosition, "Played back final position {0} did not match actual final position {1}", entity.position, finalPosition);
        Debug.LogFormat("Finished playback");

        yield break;
    }
}
