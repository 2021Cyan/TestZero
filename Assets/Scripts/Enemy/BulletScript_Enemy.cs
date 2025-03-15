using UnityEngine;

public class BulletScript_Enemy : MonoBehaviour
{
    public float speed = 50f; 
    public float damage = 5f; 
    public float lifetime = 5f; 
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = transform.right * speed;
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox_Player"))
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();

            if (player != null && !player.GetPlayerInvincible() && player.IsAlive())
            {
                player.Hurt(damage);
                Destroy(gameObject);
            }
        }
        if (other.CompareTag("Terrain"))
        {
            Destroy(gameObject);
        }
    }
}

