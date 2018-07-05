using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameCounter : MonoSingleton<FrameCounter>
{
	// The basic tick that doesn't try to be before/after anything else.
	public EventHandler OnAwake;
	public EventHandler OnStart;
    public EventHandler<int, float> OnUpdate;

	public int count { get; private set; }
    public float deltaTime { 
        get { return deltaTimes.Count > 0 ? deltaTimes[deltaTimes.Count - 1] : 0; }
        private set { deltaTime = value; }
    }
    public List<float> deltaTimes { get; private set; }

    public override void Awake() {
        base.Awake();

        deltaTimes = new List<float>();

		Events.Raise(OnAwake);
	}

    public void Update() {
		Events.Raise(OnUpdate, count, deltaTime);
    }

	public void Start() {
        EarlyUpdate.Instance.OnUpdate += HandleEarlyUpdate;
        LateUpdate.Instance.OnUpdate += HandleLateUpdate;

		Events.Raise(OnStart);
	}

	private void HandleEarlyUpdate() {
        deltaTimes.Add(Time.deltaTime);
    }

	private void HandleLateUpdate() {
		++count;
	}
}
