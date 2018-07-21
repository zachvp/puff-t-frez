﻿using UnityEngine;

// TODO: This isn't tied into replay - need to buffer input at the marionette level i think
public class PlayerHandGrenadeInputControllerKeyboard : InputController<HandGrenadeInput>
{
	private IPlayerMarionette marionette;

	public PlayerHandGrenadeInputControllerKeyboard(IPlayerMarionette marionetteInstance) {
		marionette = marionetteInstance;
	}

	public override void HandleUpdate(int currentFrame, float deltaTime) {
		base.HandleUpdate(currentFrame, deltaTime);

		input.direction |= Input.GetKey(KeyCode.RightArrow) ? Direction2D.RIGHT : 0;
		input.direction |= Input.GetKey(KeyCode.LeftArrow) ? Direction2D.LEFT : 0;

		input.launch = Input.GetKey(KeyCode.D);
        
		var snapshot = new InputSnapshot<HandGrenadeInput>(input, oldInput);

		marionette.ApplyGrenadeInput(snapshot);
	}


}
