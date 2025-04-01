using UnityEngine;
using System.Collections;

public class EnemyScript_Turret : EnemyBase
{
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

    private bool isPlayerNearby;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioManager _audio;

    [SerializeField] GameObject explosion;

    private int shotsFired = 0; 
    private float burstCooldown = 1.5f;
    private bool canShoot = true;


    void Start()
    {
        _audio = AudioManager.Instance;
        isalive = true;
        resourceAmount = 200;
        maxHealth = 200;
        currentHealth = maxHealth;
        fireRate = 0.5f;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player = playerController.GetAimPos();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        isPlayerNearby = CheckNearbyPlayers();
        ChangeDirection();
        Aim();
        Shoot();
        if (_levelNumber >= 1)
        {
            FireMissile();
        }
        if (_levelNumber >= 2)
        {
            //Zap();
        }
    }


    // Flips based on the player location
    private void ChangeDirection()
    {
        if (player == null) return;
        bool isPlayerOnRight = player.position.x > transform.position.x;

        if (isPlayerOnRight)
        {
            transform.localScale = new Vector3(1, 1, 1);
            turret.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            turret.localScale = new Vector3(-1, 1, 1);
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
            lineRenderer.enabled = false;
        }
    }

    private void Shoot()
    {
        bool isPathClear = !Physics2D.Linecast(turret_firePoint.position, player.position, LayerMask.GetMask("Terrain"));

        if (isPlayerNearby && canShoot && Time.time > lastFireTime + fireRate && isPathClear)
        {
            if (shotsFired < 3)
            {
                lastFireTime = Time.time;
                shotsFired++;

                if (turret_bullet != null && turret_firePoint != null)
                {
                    _audio.PlayOneShot(_audio.Laser, transform.position);
                    Instantiate(turret_bullet, turret_firePoint.position, turret_firePoint.rotation);
                }
            }

            if (shotsFired >= 3) 
            {
                canShoot = false; 
                StartCoroutine(BurstCooldown());
            }
        }
    }

    private IEnumerator BurstCooldown()
    {
        yield return new WaitForSeconds(burstCooldown);
        shotsFired = 0;
        canShoot = true; 
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
        isalive = false;
        lineRenderer.enabled = false;
        Vector3 explosionPos = transform.position;
        GameObject explosionInstance = Instantiate(explosion, explosionPos, Quaternion.identity);
        _audio.PlayOneShot(_audio.Explosion, transform.position);
        Destroy(explosionInstance.gameObject, 5f);
        base.Die(resourceAmount);
    }
}
