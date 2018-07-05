using UnityEngine;

// Responsible for
//    Passing input to limbs
//    Enabling/disabling limbs

public class PlayerMarionette : IPlayerMarionette
{
	private IPlayerInput playerInput;
	private ILobInput handGrenadeInput;
        
	public void AttachBody(IPlayerInput body) {
		playerInput = body;
	}

	public void AttachHandGrenade(ILobInput lobInput) {
		handGrenadeInput = lobInput;
	}

	// IPlayerMarionette begin
	public void ApplyPlayerInput(PlayerInputSnapshot snapshot)
	{
		playerInput.ApplyInput(snapshot);
	}

	public void ApplyGrenadeInput()
	{
		handGrenadeInput.Lob();
	}

	public void ApplyDeltaTime(float deltaTime)
	{
		playerInput.ApplyDeltaTime(deltaTime);
	}
	// IPlayerMarionette end
}

public interface IPlayerMarionette
{
	void ApplyPlayerInput(PlayerInputSnapshot snapshot);
	void ApplyGrenadeInput();

	void ApplyDeltaTime(float deltaTime);
}
