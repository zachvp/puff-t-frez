using UnityEngine;

public class EnemyInitializer : MonoBehaviour
{
    public PhysicsEntity enemyTemplate;

    public void Awake()
    {
        var enemy = Instantiate(enemyTemplate, transform.position, Quaternion.identity);
        var motor = new EnemyMotor(enemy, transform);
    }
}
