using UnityEngine;

public class HealthPack : Interactable
{
    // Public attributes
    public float HealAmount;

    // Behaviour
    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

        if (rb != null && other.CompareTag("Player"))
        {
            // Give health to player
            _player.GetComponent<PlayerController>().Restore(HealAmount);
            Destroy(gameObject);
        }
    }
}
