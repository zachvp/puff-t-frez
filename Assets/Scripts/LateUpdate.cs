public class LateUpdate : MonoSingleton<LateUpdate> {
    public EventHandler OnUpdate;

	public void Update() {
        Events.Raise(OnUpdate);
	}
}
