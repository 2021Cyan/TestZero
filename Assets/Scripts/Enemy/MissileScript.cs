using UnityEngine;
using System.Collections;


public class MissileScript : EnemyBase
{
    public float moveSpeed = 3f;          
    public float rotationSpeed = 200f;    
    public float explosionRadius = 5f;    
    public float damage = 10f;            
    public GameObject explosionEffect;    

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private bool isExploded = false;

    void Start()
    {
        isalive = true;
        resourceAmount = 40;
        maxHealth = 20;
        currentHealth = maxHealth;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player = playerController.GetAimPos();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isExploded || player == null || !isalive)
        {
            return;
        }
        Move();
    }

    private void Move()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float currentAngle = Mathf.LerpAngle(transform.rotation.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        rb.linearVelocity = transform.right * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isExploded)
            return;

        if (other.CompareTag("Player") || other.CompareTag("Terrain"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        isExploded = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController playerController = hit.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.Hurt(damage);
                }
            }
        }
        isalive = false;
        Destroy(gameObject);
    }

    // Function to deal damage to the enemy
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    // Function that handles the enemy death
    protected override void Die(int amount)
    {
        if (!isalive && !isExploded)
        {
            base.Die(resourceAmount);
        }
    }
}