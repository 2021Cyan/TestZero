using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

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
    private LineRenderer lineRenderer;

    // Enemy missile
    [SerializeField] GameObject missile;
    public float missileCooldown = 5f;
    private float lastMissileTime = 0f;

    private float timer;
    private bool isPlayerNearby;
    private Transform player;
    private Rigidbody2D rb;
    private bool isFalling = false;
    private SpriteRenderer spriteRenderer;
    private AudioManager _audio;

    [SerializeField] GameObject explosion;

    void Start()
    {
        _audio = AudioManager.Instance;
        isalive = true;
        resourceAmount = 320;
        maxHealth = 150;
        currentHealth = maxHealth;
        fireRate = 1.5f;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player = playerController.GetAimPos();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
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
        if(playerController.currentLevel >= 2)
        {
            FireMissile();
        }
        if(playerController.currentLevel >= 3)
        {
            //Zap();
        }
    }


    // Move to random direction
    // No parameter -> Random direction
    // collisonHappen == true -> Opposite direction
    private void ChangeDirection(bool collisonHappen = default(bool))
    {
        if (!collisonHappen)
        {
            moveDirection = Random.Range(0, 2) == 1 ? Vector3.right : Vector3.left;
        }
        else
        {
            moveDirection.x = -rb.linearVelocity.normalized.x;
        }
        transform.localScale = new Vector3(Mathf.Sign(moveDirection.x), 1, 1);
        turret.localScale = new Vector3(Mathf.Sign(moveDirection.x), 1, 1);
    }


    // Move Drone
    private void MoveDrone()
    {
        // timer += Time.deltaTime;
        // if (timer >= changeDirectionTime)
        // {
        //     ChangeDirection();
        //     // _audio.PlayOneShot(_audio.EnemyFlying, transform.position);
        //     timer = 0;
        // }
        float floatingY = Mathf.Sin(Time.time * 2f) * floatStrength;

        // RaycastHit2D wallCheck = Physics2D.Raycast(transform.position, moveDirection, 1f, LayerMask.GetMask("Terrain"));
        // if (wallCheck.collider != null)
        // {
        //     ChangeDirection();
        //     // _audio.PlayOneShot(_audio.EnemyFlying, transform.position);
        // }
        rb.linearVelocity = new Vector3(moveDirection.x, floatingY, 0) * moveSpeed;
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
            ChangeDirection(true);
        }
    }

    private bool CheckNearbyPlayers()
    {
        if (player == null)
        {
            return false;
        }
        bool isPathClear = !Physics2D.Linecast(transform.position, player.position, LayerMask.GetMask("Terrain"));
        return Vector3.Distance(transform.position, player.position) <= detectionRange && isPathClear;
    }

    private void Aim()
    {
        if (isPlayerNearby)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, turret_firePoint.position);
            lineRenderer.SetPosition(1, player.position);
            Vector3 direction = (player.position - turret.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            turret.rotation = Quaternion.Euler(0, 0, angle + 45);
        }
        else
        {
            lineRenderer.enabled=false;
        }
    }

    private void Shoot()
    {
        bool isPathClear = !Physics2D.Linecast(turret_firePoint.position, player.position, LayerMask.GetMask("Terrain"));
        if (isPlayerNearby && Time.time > lastFireTime + fireRate && isPathClear)
        {
            lastFireTime = Time.time;
            if (turret_bullet != null && turret_firePoint != null)
            {
                _audio.PlayOneShot(_audio.Laser, transform.position);
                Instantiate(turret_bullet, turret_firePoint.position, turret_firePoint.rotation);
            }
        }
    }

    private void FireMissile()
    {
        if (Time.time > lastMissileTime + missileCooldown)
        {
            bool isPathClear = !Physics2D.Linecast(turret_firePoint.position, player.position, LayerMask.GetMask("Terrain"));
            if (isPlayerNearby && player != null && isPathClear)
            {
                lastMissileTime = Time.time;
                Instantiate(missile, turret_firePoint.position, turret_firePoint.rotation);
            }
        }
    }

    // Function to deal damage to the enemy
    public override void TakeDamage(float damage)
    {
        if (!isalive)
        {
            return;
        }

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
            isalive = false;
            lineRenderer.enabled = false;
            rb.gravityScale = 1f;
            rb.constraints = RigidbodyConstraints2D.None;
            float randomTorque = Random.Range(-5f, 5f);
            rb.AddTorque(randomTorque, ForceMode2D.Impulse);
        }
    }
}
