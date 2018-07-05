using UnityEngine;

public class LobMotorData : ScriptableObject
{
	private const float DIRECTION_X = 1;
	private const float DIRECTION_Y = 0.6f;

	public int speed = 1800;
    public int forceFrameLength = 8;
    public int gravity = 80;

	public Vector3 directionRight = new Vector3(DIRECTION_X, DIRECTION_Y, 0).normalized;
	public Vector3 directionLeft = new Vector3(-DIRECTION_X, DIRECTION_Y, 0).normalized;
}
