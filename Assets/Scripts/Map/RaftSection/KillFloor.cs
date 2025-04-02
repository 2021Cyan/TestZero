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
            other.gameObject.GetComponent<EnemyBase>().Smite();
        }
    }
}
