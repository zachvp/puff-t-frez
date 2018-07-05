using UnityEngine;

// TODO: This isn't tied into replay - need to buffer input at the marionette level i think
public class PlayerHandGrenadeInputControllerKeyboard {
	private ILobInput lob;

	public PlayerHandGrenadeInputControllerKeyboard(ILobInput lobInput) {
		lob = lobInput;

		FrameCounter.Instance.OnUpdate += HandleUpdate;
	}

	public void HandleUpdate(int currentFrame, float deltaTime) {
		if (Input.GetKeyDown(KeyCode.D)) {
			lob.Lob();
		}
	}
}
