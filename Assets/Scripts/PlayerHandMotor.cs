using UnityEngine;

// TODO: Remove this
public class PlayerHandMotor :
    IdleLimbMotor<IdleLimbMotorData>,
    ICoreInput<HandInput>
{
	// TODO: Move this field to super class?
	private InputSnapshot<HandInput> input;
	private Vector3 anchorPosition;

	public PlayerHandMotor(Entity e, Transform t)
		: base(e, t)
	{
		input = new InputSnapshot<HandInput>();
	}

	// TODO: Can probly remove
	//public override void HandleUpdate(long currentFrame, float deltaTime)
	//{
	//	base.HandleUpdate(currentFrame, deltaTime);
        

	//	anchorPosition = base.GetRootPosition();
        
	//	if (FlagsHelper.IsSet(input.held.direction.Flags, Direction2D.HORIZONTAL))
 //       {
	//		var directionFactor = Mathf.Sign(input.held.direction.Vector.x);

 //           anchorPosition.x += directionFactor * data.faceOffset;

	//		Debug.LogFormat("influencing hand anchor - directionFactor: {0}\toffset: {1}", directionFactor, data.faceOffset);
 //       }
	//}

	//protected override Vector3 GetRootPosition()
	//{
	//	return anchorPosition;
	//}

	// ICoreInput
	public void ApplyInput(InputSnapshot<HandInput> snapshot)
	{
		input = snapshot;
	}
}
