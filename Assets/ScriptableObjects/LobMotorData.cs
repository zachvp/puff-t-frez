using UnityEngine;

public class LobMotorData : ScriptableObject
{
	public int speed = 1800;
    public int forceFrameLength = 8;
    public int gravity = 80;

	public Vector3 multiplier = new Vector3(1, 0.8f, 0);
}
