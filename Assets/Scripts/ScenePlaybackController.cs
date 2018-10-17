using System.Collections.Generic;
using UnityEngine;

// todo: rename to SceneRecorder
public class ScenePlaybackController
{
    // Map of IDs to Entity objects
    private HashSet<Entity> entities;
    private Dictionary<long, CoreTransform> transforms;

    public ScenePlaybackController()
    {
        entities = new HashSet<Entity>();
        transforms = new Dictionary<long, CoreTransform>();
    }

    public void Register(Entity entity)
    {
        Debug.AssertFormat(!entities.Contains(entity), "Attempting to register an entity that's already been registered");

        // Register the entity and snapshot its position for the current frame.
        entities.Add(entity);
        transforms.Add(entity.id, new CoreTransform(entity.transform));
    }

    public void Reset()
    {
        foreach (Entity e in entities)
        {
            try
            {
                var t = transforms[e.id];

                e.SetTransform(t);
            }
            catch(KeyNotFoundException)
            {
                Debug.LogErrorFormat("key id {0} not found in transforms", e.id);
            }
        }
    }
}
