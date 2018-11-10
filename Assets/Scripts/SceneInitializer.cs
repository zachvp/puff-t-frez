﻿using UnityEngine;
using UnityEngine.SceneManagement;

// todo: rename to DebugController or sth
public class SceneInitializer : MonoBehaviour
{
    ScenePlaybackController playback;

    public void Awake()
    {
        playback = new ScenePlaybackController();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var scene = SceneManager.GetActiveScene();

            SceneManager.LoadScene(scene.buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {

        }
    }

    public void OnEntityCreate(Entity e)
    {
        playback.Register(e);
    }
}
