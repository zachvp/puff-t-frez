using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs
public class PlayerMarionette {
	public IPlayerInput playerInput;
    
	// TODO: Remove plan
    // Player character init
	//    player marionette(ILimb0, iLimb1, ...)
    //        limb 0
    //        limb 1
    //        ....

	public PlayerMarionette(IPlayerInput inPlayerInput) {
		playerInput = inPlayerInput;
	}
}
