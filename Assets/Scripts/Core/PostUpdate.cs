public class PostUpdate : MonoSingleton<PostUpdate>
{
    public EventHandler OnUpdate;

	public void Update()
	{
        Events.Raise(OnUpdate);
	}
}
