using System;
using UnityEngine;

public class FrameCounter : MonoSingleton<FrameCounter>
{
	public int count { get; private set; }

	public void LateUpdate()
	{
		++count;
	}
}
