using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameCounter : MonoSingleton<FrameCounter>
{
	// The basic tick that doesn't try to be before/after anything else.
	public EventHandler OnAwake;
	public EventHandler OnStart;
    public EventHandler<long, float> OnUpdate;
    public EventHandler<float> OnFixedUpdate;
	public EventHandler OnLateUpdate;

	public int count { get; private set; }

	public List<float> deltaTimes { get; private set; }
    public float deltaTime
	{
        get { return deltaTimes[deltaTimes.Count - 1]; }
        private set { deltaTime = value; }
    }

    public override void Awake()
	{
        base.Awake();

        deltaTimes = new List<float>();
        deltaTimes.Add(0);

		Events.Raise(OnAwake);
	}

    public void Update()
	{
		Events.Raise(OnUpdate, count, deltaTime);
    }

    public void FixedUpdate()
    {
        Events.Raise(OnFixedUpdate, Time.fixedDeltaTime);
    }

    public void LateUpdate()
	{
		Events.Raise(OnLateUpdate);
	}

	public void Start()
	{
        PreUpdate.Instance.OnUpdate += HandleEarlyUpdate;
        PostUpdate.Instance.OnUpdate += HandlePostUpdateGlobal;

		Events.Raise(OnStart);
	}

	public void HandleEarlyUpdate()
	{
        deltaTimes.Add(Time.deltaTime);
    }

	public void HandlePostUpdateGlobal()
	{
		++count;
	}
}
