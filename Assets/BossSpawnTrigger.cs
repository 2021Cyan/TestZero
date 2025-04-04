using UnityEngine;

public class BossSpawnTrigger : MonoBehaviour
{
    // Attributes
    public GameObject BossEnemy;
    public Transform BossSpawnPoint;
    private bool hasSpawned = false;

    // Behaviour
    void OnTriggerEnter2D(Collider2D other)
    {
        // Spawn boss if player enters hitbox
        if (other.CompareTag("Player") && !hasSpawned)
        {
            Instantiate(BossEnemy, BossSpawnPoint.position, Quaternion.identity);
            hasSpawned = true;
        }
    }
}
