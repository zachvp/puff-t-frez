public class EarlyUpdate : MonoSingleton<EarlyUpdate> {
    public EventHandler OnUpdate;

	public void Update() {
        Events.Raise(OnUpdate);
	}
}
