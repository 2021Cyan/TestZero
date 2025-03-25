using UnityEngine;

public class KillFloor : MonoBehaviour
{
    // Behaviour
    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null && other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().Hurt(float.MaxValue);
        }

        else if (rb != null && other.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyBase>().TakeDamage(float.MaxValue);
        }
    }
}
