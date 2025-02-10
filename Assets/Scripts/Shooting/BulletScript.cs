using UnityEngine;

public class BulletScript : MonoBehaviour
{
    // Speed of the bullet
    public float speed = 50f;
    // Lifetime of the bullet
    public float lifetime = 10f;
    // Rigidbody of the bullet
    private Rigidbody2D rb;
    // Damage of the bullet
    public float damage = 10f;

    void Start()
    {
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        // Calculate the direction based on the rotation angle
        float angle = transform.eulerAngles.z;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));


        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Adjust direction based on the mouse position
        if (mousePos.x < transform.position.x)
        {
            direction = -direction;
        }

        // Set the velocity of the bullet in the calculated direction
        rb.linearVelocity = direction * speed;

        // Destroys the bullet after the lifetime
        Destroy(gameObject, lifetime);
    }

    // Collision detection with terrain
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Contact with the terrain (Wall, ground, obstacles...etc)
        if (other.gameObject.CompareTag("Terrain"))
        {
            // Disable the bullet object
            gameObject.SetActive(false);
        }

        // Contact with the Enemy 
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Get enemy component
            Enemy enemy = other.GetComponent<Enemy>();

            // Enemy is not null
            if (enemy != null)
            {
                // Inflict the damage to the enemy 
                enemy.TakeDamage((int)damage);
                // Disable the bullet object
                gameObject.SetActive(false);
            }
        }
    }
}
