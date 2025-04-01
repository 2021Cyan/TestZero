using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public GameObject[] Enemies;

    public void Spawn(int levelNumber)
    {
        // Spawn enemy into scene
        GameObject enemy = Enemies[Random.Range(0, Enemies.Length)];
        enemy = Instantiate(enemy, transform.position, Quaternion.identity);

        // Convey level to enemy script
        enemy.GetComponent<EnemyBase>().SetLevelNumber(levelNumber);        
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
