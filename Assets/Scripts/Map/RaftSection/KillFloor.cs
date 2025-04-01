using UnityEngine;

public class KillFloor : MonoBehaviour
{
    // Behaviour
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().Hurt(float.MaxValue);
        }

        else if (other.CompareTag("Enemy"))
        {
            EnemyBase enemy = other.gameObject.GetComponent<EnemyBase>();
            enemy.ZeroResourceAmount();
            enemy.TakeDamage(float.MaxValue);
        }
    }
}
