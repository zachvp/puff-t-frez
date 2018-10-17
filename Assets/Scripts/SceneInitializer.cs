using UnityEngine;
using UnityEngine.SceneManagement;

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
    }

    public void OnEntityCreate(Entity e)
    {
        playback.Register(e);
    }
}
