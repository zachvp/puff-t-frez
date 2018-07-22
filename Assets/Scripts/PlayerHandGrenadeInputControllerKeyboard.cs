using UnityEngine;

// TODO: This isn't tied into replay - need to buffer input at the marionette level i think
public class PlayerHandGrenadeInputControllerKeyboard : InputController<HandGrenadeInput, IPlayerMarionette>
{

	public PlayerHandGrenadeInputControllerKeyboard(IPlayerMarionette m)
		: base(m)
	{ }

	public override void HandleUpdate(long currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

		FlagsHelper.Set(ref input.direction,
		                Direction2D.RIGHT,
		                Input.GetKey(KeyCode.RightArrow));
		FlagsHelper.Set(ref input.direction,
		                Direction2D.LEFT,
		                Input.GetKey(KeyCode.LeftArrow));

		input.launch = Input.GetKey(KeyCode.D);
        
		if (FlagsHelper.IsSet(input.direction, Direction2D.HORIZONTAL, true))
		{
			FlagsHelper.Unset(ref input.direction, Direction2D.HORIZONTAL);
		}

		Debug.AssertFormat(!FlagsHelper.IsSet(input.direction,
		                                      Direction2D.LEFT | Direction2D.RIGHT,
		                                      true),
		                   "Invalid direction given: {0}",
		                   input.direction);

		var snapshot = new InputSnapshot<HandGrenadeInput>(oldInput, input);

		responder.ApplyGrenadeInput(snapshot);
	}


}
