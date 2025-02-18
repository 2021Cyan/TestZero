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
    public float damage = 0f;
    private InputManager _Input;
    private Transform player;
    private PlayerController playerController;
    public GameObject damageTextPrefab;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                damage = playerController.damage;
            }
        }

    }
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        damage = playerController.damage;
        _Input = InputManager.Instance;
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        // Calculate the direction based on the rotation angle
        float angle = transform.eulerAngles.z;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));


        Vector3 mousePos = Camera.main.ScreenToWorldPoint(_Input.MouseInput);
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
                enemy.TakeDamage((int)damage);
                Vector3 hitPosition = other.ClosestPoint(transform.position);
                ShowDamageText((int)damage, hitPosition);
                gameObject.SetActive(false);
            }
        }
    }

    private void ShowDamageText(int damageAmount, Vector3 position)
    {
        if (damageTextPrefab != null)
        {
            GameObject damageText = Instantiate(damageTextPrefab, position, Quaternion.identity);
            DamageText textComponent = damageText.GetComponent<DamageText>();
            if (textComponent != null)
            {
                textComponent.SetDamageText(damageAmount);
            }
        }
    }
}
