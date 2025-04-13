using UnityEngine;

public class HealthPack : Interactable
{
    // Public attributes
    public float HealAmount;
    public float Likelihood = 1f;

    // Behaviour
    void Awake()
    {
        // Destory self based on likelihood
        if (Random.value > Likelihood)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Give health to player
            _player.GetComponent<PlayerController>().Restore(HealAmount);
            Destroy(gameObject);
        }
    }
}
