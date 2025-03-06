using UnityEngine;

public class BulletScript_Pod : MonoBehaviour
{
    // Hitmarker references
    public GameObject hitmarkerPrefab;
    // Speed of the bullet
    public float speed = 50f;
    // Lifetime of the bullet
    public float lifetime = 5f;
    // Rigidbody of the bullet
    private Rigidbody2D rb;
    // Damage of the bullet
    public float damage = 0f;
    private InputManager _input;
    public GameObject damageTextPrefab;

    void Start()
    {
        if (PodScript.Instance != null) 
        {
            damage = damage += (PodScript.Instance.weaponlevel - 1) * 1; 
        }

        _input = InputManager.Instance;
        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
        // Calculate the direction based on the rotation angle
        float angle = transform.eulerAngles.z;
        Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
        // Set initial velocity and initialize homing direction
        rb.linearVelocity = direction * speed;
        Destroy(gameObject, lifetime);
    }
    private void ShowHitmarker()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(_input.MouseInput);
        mousePos.z = 0;
        if (hitmarkerPrefab != null)
        {
            GameObject hitmarker = Instantiate(hitmarkerPrefab, mousePos, Quaternion.identity);
            Destroy(hitmarker, 0.05f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Contact with the Enemy 
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Get enemy component
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            // Enemy is not null
            if (enemy != null)
            {
                enemy.TakeDamage((int)damage);
                Vector3 hitPosition = other.ClosestPoint(transform.position);
                ShowDamageText((int)damage, hitPosition);
                ShowHitmarker();
            }
            gameObject.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        Vector2 moveDirection = rb.linearVelocity.normalized;
        float moveDistance = speed * Time.fixedDeltaTime;
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, moveDirection, moveDistance, LayerMask.GetMask("Terrain"));
        if (wallHit.collider != null)
        {
            transform.position = wallHit.point + wallHit.normal * 0.1f;
            gameObject.SetActive(false);
        }
    }

    // Shows damage text prefab when hitting enemy
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
