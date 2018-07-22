using UnityEngine;

// TODO: This isn't tied into replay - need to buffer input at the marionette level i think
public class PlayerHandGrenadeInputControllerKeyboard : InputController<HandGrenadeInput>
{
	private IPlayerMarionette marionette;

	public PlayerHandGrenadeInputControllerKeyboard(IPlayerMarionette marionetteInstance) {
		marionette = marionetteInstance;
	}

	public override void HandleUpdate(long currentFrame, float deltaTime) {
		base.HandleUpdate(currentFrame, deltaTime);

		input.direction |= Input.GetKey(KeyCode.RightArrow) ? Direction2D.RIGHT : Direction2D.NONE;
		input.direction |= Input.GetKey(KeyCode.LeftArrow) ? Direction2D.LEFT : Direction2D.NONE;

		input.launch = Input.GetKey(KeyCode.D);
        
		var snapshot = new InputSnapshot<HandGrenadeInput>(input, oldInput);

		marionette.ApplyGrenadeInput(snapshot);
	}


}
