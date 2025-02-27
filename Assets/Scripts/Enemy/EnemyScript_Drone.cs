using UnityEngine;
using System.Collections;

public class Enemy_Drone : EnemyBase
{
    // Enemy movement
    private Vector3 moveDirection;
    public float moveSpeed = 3f;
    public float changeDirectionTime = 2f;
    public float floatStrength = 0.01f;

    // Enemy shooting
    [SerializeField] Transform turret;
    [SerializeField] Transform turret_firePoint;
    [SerializeField] GameObject turret_bullet;
    public float fireRate;
    private float lastFireTime = 0f;
    public float detectionRange = 15;

    private float timer;
    private bool isPlayerNearby;
    private Transform player;
    private Rigidbody2D rb;
    private bool isFalling = false;
    private SpriteRenderer spriteRenderer;
    private AudioManager _audio;

    [SerializeField] ParticleSystem muzzleFlash_fire;
    [SerializeField] GameObject explosion;

    void Start()
    {
        _audio = AudioManager.Instance;
        isalive = true;
        resourceAmount = 200;
        maxHealth = 150;
        currentHealth = maxHealth;
        fireRate = 1.5f;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player = playerController.GetAimPos();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChangeDirection();
    }

    void Update()
    {
        if (isFalling)
        {
            return;
        }
        MoveDrone();
        isPlayerNearby = CheckNearbyPlayers();
        Aim();
        Shoot();
    }


    // Move to random direction
    private void ChangeDirection()
    {
        float randomX = Random.Range(-1f, 1f);
        moveDirection = new Vector3(randomX, 0, 0).normalized;
    }


    // Move Drone
    private void MoveDrone()
    {
        timer += Time.deltaTime;
        if (timer >= changeDirectionTime)
        {
            ChangeDirection();
            _audio.PlayOneShot(_audio.EnemyFlying, transform.position);
            timer = 0;
        }
        float floatingY = Mathf.Sin(Time.time * 2f) * floatStrength;

        RaycastHit2D wallCheck = Physics2D.Raycast(transform.position, moveDirection, 1f, LayerMask.GetMask("Terrain"));
        if (wallCheck.collider != null)
        {
            ChangeDirection();
            _audio.PlayOneShot(_audio.EnemyFlying, transform.position);
        }
        rb.linearVelocity = (moveDirection + new Vector3(0, floatingY, 0)) * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isFalling && other.CompareTag("Terrain"))
        {
            Vector3 explosionPos = transform.position + new Vector3(0, -1f, 0);
            GameObject explosionInstance = Instantiate(explosion, explosionPos, Quaternion.identity);
            _audio.PlayOneShot(_audio.Explosion, transform.position);
            Destroy(explosionInstance.gameObject, 5f);
            base.Die(resourceAmount);
        }
        else if (!isFalling && other.CompareTag("Terrain"))
        {
            ChangeDirection();
        }
    }

    private bool CheckNearbyPlayers()
    {
        if (player == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, player.position) <= detectionRange;
    }

    private void Aim()
    {
        if (isPlayerNearby && player != null)
        {
            Vector3 direction = (player.position - turret.position).normalized;
            Debug.DrawLine(turret.position, turret.position + direction * 3f, Color.red, 0.1f);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void Shoot()
    {
        if (isPlayerNearby && Time.time > lastFireTime + fireRate)
        {
            lastFireTime = Time.time;

            if (muzzleFlash_fire != null)
            {
                muzzleFlash_fire.Play();
            }
            if (turret_bullet != null && turret_firePoint != null)
            {
                _audio.PlayOneShot(_audio.Laser, transform.position);
                Instantiate(turret_bullet, turret_firePoint.position, turret_firePoint.rotation);
            }
        }
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
        if (!isFalling)
        {
            isFalling = true;
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.None;
            float randomTorque = Random.Range(-5f, 5f);
            rb.AddTorque(randomTorque, ForceMode2D.Impulse);
        }
    }
}
