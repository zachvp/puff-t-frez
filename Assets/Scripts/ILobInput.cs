// TODO: Might make sense to have this be general, one-off and freeze input
public interface ILobInput {
    // TODO: Add params?
	void Lob(Direction2D direction);
    void Freeze();
	void Reset();
}
