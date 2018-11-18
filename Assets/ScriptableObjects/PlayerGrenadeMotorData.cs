public class PlayerGrenadeMotorData : LobMotorData
{
	public int frameDelayReset = 64;
	public int inputCountLob = 4;
	public float lobVelocityCoefficient = 0.2f;

    public void Awake()
    {
        speed = 1800;
    }
}
