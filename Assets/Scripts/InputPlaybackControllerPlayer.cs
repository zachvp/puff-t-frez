using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Records and plays back player input
// TODO: Make generic so can be done for all input types
public class InputPlaybackControllerPlayer
{
	public static EventHandler OnPlaybackStarted;

	// TODO: Replace this with PlayerMotor
	private ICoreInput<PlayerInput> player;
	private Entity entity;

	private InputBuffer<InputSnapshot<PlayerInput>> buffer;
    private Vector3 initialPosition;
    
//#if DEBUG
    private Vector3 finalPosition;
//#endif
    
	public InputPlaybackControllerPlayer(ICoreInput<PlayerInput> playerMotor,
	                                     Entity playerTransform,
	                                     InputBuffer<InputSnapshot<PlayerInput>> inputBuffer)
	{
		player = playerMotor;
		entity = playerTransform;
		buffer = inputBuffer;

		initialPosition = entity.Position;

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public void HandleUpdate(long currentFrame, float deltaTime)
	{
        if (Input.GetKeyDown(KeyCode.R)) {
			Events.Raise(OnPlaybackStarted);

			finalPosition = entity.Position;

			entity.SetPosition(initialPosition);
			CoreBehaviour.Instance.StartCoroutine(PlaybackFrames());
        }
	}

	// TODO: This is slightly off. Think it has to do with the frame buffer count mismatch.
    private IEnumerator PlaybackFrames()
	{
		var bufferCopy = new List<InputSnapshot<PlayerInput>>(buffer.inputBuffer);
        var timeCopy = new List<float>(FrameCounter.Instance.deltaTimes);
        var i = 0;

        Debug.AssertFormat(bufferCopy.Count == timeCopy.Count, "Input buffer length {0} different from DeltaTime length {1}", bufferCopy.Count, timeCopy.Count);

		foreach (InputSnapshot<PlayerInput> input in bufferCopy) {
            var deltaTime = timeCopy[i];

            player.ApplyInput(input);
            i++;
            yield return null;
        }

		Debug.AssertFormat(entity.Position == finalPosition, "Played back final position {0} did not match actual final position {1}", entity.Position, finalPosition);
        Debug.LogFormat("Finished playback");

        yield break;
    }
}
