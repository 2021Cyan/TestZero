using UnityEngine;

public class BossSpawnTrigger : MonoBehaviour
{
    // Attributes
    public GameObject BossEnemy;
    public Transform BossSpawnPoint;

    // Behaviour
    void OnTriggerEnter2D(Collider2D other)
    {
        // Spawn boss if player enters hitbox
        if (other.CompareTag("Player"))
        {
            Instantiate(BossEnemy, BossSpawnPoint.position, Quaternion.identity);
        }
    }
}
