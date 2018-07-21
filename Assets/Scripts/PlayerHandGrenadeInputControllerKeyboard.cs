using UnityEngine;

// TODO: This isn't tied into replay - need to buffer input at the marionette level i think
public class PlayerHandGrenadeInputControllerKeyboard {
	private IPlayerMarionette marionette;

	public PlayerHandGrenadeInputControllerKeyboard(IPlayerMarionette marionetteInstance) {
		marionette = marionetteInstance;

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public void HandleUpdate(int currentFrame, float deltaTime) {
		var input = new HandGrenadeInput();

		if (Input.GetKeyDown(KeyCode.D)) {
			// TODO: Fix
			//marionette.ApplyGrenadeInput();
		}
	}
}
