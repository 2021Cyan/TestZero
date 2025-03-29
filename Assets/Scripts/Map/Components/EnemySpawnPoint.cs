using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public GameObject[] Enemies;

    public void Spawn(int levelNumber)
    {
        // Spawn enemy into scene
        Instantiate(Enemies[Random.Range(0, Enemies.Length)], transform.position, Quaternion.identity);

        // Convey level to enemy script
        // TODO
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
