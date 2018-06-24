﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameCounter : MonoSingleton<FrameCounter>
{
	public int count { get; private set; }
    public float deltaTime { 
        get { return deltaTimes.Count > 0 ? deltaTimes[deltaTimes.Count - 1] : 0; }
        private set { deltaTime = value; }
    }
    public List<float> deltaTimes { get; private set; }

    public override void Awake() {
        base.Awake();

        deltaTimes = new List<float>();
	}

	public void Start() {
        EarlyUpdate.Instance.OnUpdate += HandleEarlyUpdate;
        LateUpdate.Instance.OnUpdate += HandleLateUpdate;
	}

	private void HandleEarlyUpdate() {
        deltaTimes.Add(Time.deltaTime);
    }

	private void HandleLateUpdate() {
		++count;
	}
}
