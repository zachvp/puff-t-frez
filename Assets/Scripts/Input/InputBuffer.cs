using UnityEngine;
using System;
using System.Collections.Generic;

public class InputBuffer<T>
{
    public List<T> inputBuffer { get; private set; }

	public InputBuffer()
	{
        inputBuffer = new List<T> ();
	}

    public void AddInput(T input)
	{
		inputBuffer.Add (input);
	}
}
