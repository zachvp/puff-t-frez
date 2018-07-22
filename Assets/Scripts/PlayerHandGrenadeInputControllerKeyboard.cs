using UnityEngine;

// TODO: This isn't tied into replay - need to buffer input at the marionette level i think
public class PlayerHandGrenadeInputControllerKeyboard : InputController<HandGrenadeInput>
{
	private IPlayerMarionette marionette;

	public PlayerHandGrenadeInputControllerKeyboard(IPlayerMarionette m)
	{
		marionette = m;
	}

	public override void HandleUpdate(long currentFrame, float deltaTime)
	{
		base.HandleUpdate(currentFrame, deltaTime);

		if (Input.GetKey(KeyCode.RightArrow))
		{
			FlagsHelper.Set(ref input.direction, Direction2D.RIGHT);
		}
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			FlagsHelper.Set(ref input.direction, Direction2D.LEFT);
		}

		input.launch = Input.GetKey(KeyCode.D);
        
		if (FlagsHelper.IsSet(input.direction, Direction2D.RIGHT) &&
		    FlagsHelper.IsSet(input.direction, Direction2D.LEFT))
		{
			FlagsHelper.Unset(ref input.direction, Direction2D.RIGHT);
			FlagsHelper.Unset(ref input.direction, Direction2D.LEFT);
		}

		Debug.AssertFormat(!(FlagsHelper.IsSet(input.direction, Direction2D.LEFT) &&
		                     FlagsHelper.IsSet(input.direction, Direction2D.RIGHT)),
		                   "Invalid direction given: {0}", input.direction);

		var snapshot = new InputSnapshot<HandGrenadeInput>(input, oldInput);

		marionette.ApplyGrenadeInput(snapshot);
	}


}
