using UnityEngine;
using System.Collections;


public class MissileScript : EnemyBase
{
    public float moveSpeed = 3f;          
    public float rotationSpeed = 200f;    
    public float explosionRadius = 5f;    
    public float damage = 10f;            
    public GameObject explosionEffect;
    public GameObject destroyEffect;
    public GameObject playerlock;

    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioManager _audio;
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
        _audio = AudioManager.Instance;
    }

    void Update()
    {
        if (isExploded || player == null || !isalive)
        {
            return;
        }
        UpdatePlayerLock();
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

    private void UpdatePlayerLock()
    {
        if (playerlock != null && playerController != null)
        {
            Vector3 playerPos = player.transform.position; 
            Vector3 missilePos = transform.position;
            playerlock.transform.position = playerPos;

            playerlock.transform.Rotate(Vector3.forward * 100f * Time.deltaTime); 
            float distance = Vector3.Distance(missilePos, playerPos);
            float scaleFactor = Mathf.Clamp(distance * 0.1f, 0.3f, 5f); 
            playerlock.transform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isExploded)
            return;

        if (playerController == null)
        {
            return;
        }

        bool isPlayerHitbox = other.gameObject.layer == LayerMask.NameToLayer("Hitbox_Player");

        if (((isPlayerHitbox) &&
            !playerController.GetPlayerInvincible() &&
            playerController.IsAlive())|| 
            other.CompareTag("Terrain"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (isExploded){
            return;
        }

        isExploded = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            _audio.PlayOneShot(_audio.Missile, transform.position);
        }

        if (playerlock != null)
        {
            Destroy(playerlock); 
        }

        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (Collider2D hit in hitObjects)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerController playerController = hit.GetComponent<PlayerController>();
                if (playerController != null && !playerController.GetPlayerInvincible() && playerController.IsAlive())
                {
                    playerController.Hurt(damage);
                }
                break;
            }
        }
        isalive = false;
        Destroy(gameObject);
    }

    private void Explode_destroy()
    {
        if (isExploded)
        {
            return;
        }

        isExploded = true;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
            _audio.PlayOneShot(_audio.Missile, transform.position);
        }

        if (playerlock != null)
        {
            Destroy(playerlock);
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
        if (!isExploded) 
        {
            Explode_destroy();  
        }
        base.Die(resourceAmount);
    }
}