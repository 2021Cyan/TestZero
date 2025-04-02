using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public GameObject[] Enemies;

    public EnemyBase Spawn(int levelNumber)
    {
        // Spawn enemy into scene
        GameObject enemy = Enemies[Random.Range(0, Enemies.Length)];
        enemy = Instantiate(enemy, transform.position, Quaternion.identity);

        // Convey level to enemy script
        EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
        enemyScript.SetLevelNumber(levelNumber);

        // Return enemy script   
        return enemyScript;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
