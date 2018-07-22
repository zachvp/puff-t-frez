public class PreUpdate : MonoSingleton<PreUpdate> {
    public EventHandler OnUpdate;

	public void Update() {
        Events.Raise(OnUpdate);
	}
}
