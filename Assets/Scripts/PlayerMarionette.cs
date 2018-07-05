using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
public class PlayerMarionette : IPlayerMarionette {
	private IPlayerInput playerInput;
	private ILobInput handGrenadeInput;

	private Entity handGrenadeTemplate;
    
	// TODO: Remove plan
    // Player character init
	//    player marionette(ILimb0, iLimb1, ...)
    //        limb 0
    //        limb 1
    //        ....

	public PlayerMarionette(IPlayerInput inPlayerInput, ILobInput inHandGrenadeInput) {
		playerInput = inPlayerInput;
		handGrenadeInput = inHandGrenadeInput;
	}

	public void ApplyPlayerInput(PlayerInputSnapshot snapshot) {
		playerInput.ApplyInput(snapshot);
	}

	public void ApplyHandInput() {
		handGrenadeInput.Lob();
	}

	public void ApplyDeltaTime(float deltaTime) {
		playerInput.ApplyDeltaTime(deltaTime);
	}
}

public interface IPlayerMarionette {
	void ApplyPlayerInput(PlayerInputSnapshot snapshot);
	void ApplyHandInput();

	void ApplyDeltaTime(float deltaTime);
}
