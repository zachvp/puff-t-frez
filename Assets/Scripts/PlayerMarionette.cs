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

	// TODO: Consider reworking the names of these params
	public PlayerMarionette(IPlayerInput inPlayerInput, ILobInput lobInput) {
		playerInput = inPlayerInput;
		handGrenadeInput = lobInput;

		// TODO: Spawn hand grenade and deactivate it.
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
