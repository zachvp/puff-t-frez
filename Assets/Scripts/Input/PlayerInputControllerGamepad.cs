﻿using InControl;
using UnityEngine;

public class PlayerInputControllerGamepad : InputController<PlayerInput, PlayerMarionette>
{
	public PlayerInputControllerGamepad(ICoreInput<PlayerInput> r,
	                                    InputBuffer<InputSnapshot<PlayerInput>> b)
		: base(r, b)
	{ }

    protected override void UpdateInput()
    {
        var device = InputManager.Devices[0];
        var crouchThreshold = 0.7f;
        var leftStick = device.LeftStick.Value;
        var dPad = device.DPad.Value;

        // TODO: Put in utility class
        var threshold = 0.2f;
        var newDir = input.direction.Vector;

        // Analog stick directions
        if (Mathf.Abs(leftStick.x) > threshold)
        {
            newDir.x = leftStick.x;
        }
        if (Mathf.Abs(leftStick.y) > threshold)
        {
            newDir.y = leftStick.y;
        }

        // DPad directions
        newDir = dPad;

        input.direction.Update(newDir);
        input.direction.CardinalizeVector();

        input.crouch = leftStick.y < -crouchThreshold || dPad.y < 0;
        input.jump = device.Action1.IsPressed;

		input.direction.ClearConcurrent();
    }
}
