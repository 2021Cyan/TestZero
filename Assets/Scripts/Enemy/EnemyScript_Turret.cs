using UnityEngine;
using System.Collections;
using FMOD.Studio;
using FMODUnity;

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
    private bool shouldFireMissile = true;

    // Enemy Zap
    [SerializeField] Transform laserOrigin;
    [SerializeField] LineRenderer laserLine;
    [SerializeField] float laserLength = 50f;
    [SerializeField] private GameObject zapEffectPrefabSingle;
    private GameObject sweepEffect;

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
        if (zapEffectPrefabSingle != null)
        {
            sweepEffect = Instantiate(zapEffectPrefabSingle, Vector3.zero, Quaternion.identity);
            sweepEffect.SetActive(false);
        }
    }

    void Update()
    {
        isPlayerNearby = CheckNearbyPlayers();
        ChangeDirection();
        Aim();
        Shoot();
        if (_levelNumber == 1)
        {
            FireMissile();
        }
        else if (_levelNumber >= 2)
        {
            AlternatingAttack();
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
        if (player == null)
        {
            return;
        }

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
        if (player == null)
        {
            return;
        }
        bool isPathClear = !Physics2D.Linecast(turret_firePoint.position, player.position, LayerMask.GetMask("Terrain"));

        if (isPlayerNearby && canShoot && Time.time > lastFireTime + fireRate && isPathClear)
        {
            if (shotsFired < 3)
            {
                lastFireTime = Time.time;
                shotsFired++;

                if (turret_bullet != null && turret_firePoint != null)
                {
                    Instantiate(turret_bullet, turret_firePoint.position, turret_firePoint.rotation);
                    _audio.PlayOneShot(_audio.Laser, transform.position);
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
        if (player == null)
        {
            return;
        }
        if (Time.time > lastMissileTime + missileCooldown)
        {
            bool isPathClear = !Physics2D.Linecast(turret_firePoint.position, player.position, LayerMask.GetMask("Terrain"));
            if (isPlayerNearby && player != null && isPathClear)
            {
                lastMissileTime = Time.time;
                Instantiate(missile, turret_firePoint.position, turret_firePoint.rotation);
                _audio.PlayOneShot(_audio.MissileLaunch, turret_firePoint.position);
            }
        }
    }

    private IEnumerator Zap()
    {
        float startAngle, endAngle;

        bool isPlayerOnLeft = player.position.x < transform.position.x;

        if (isPlayerOnLeft)
        {
            startAngle = -180f;
            endAngle = 0f;
        }
        else
        {
            startAngle = 180f;
            endAngle = 0f;
        }

        float currentAngle = startAngle;
        float totalAngle = Mathf.Abs(endAngle - startAngle);
        float traveledAngle = 0f;

        float startSpeed = 50f;
        float endSpeed = 300f;

        bool hasHitPlayer = false;

        laserLine.enabled = true;

        Vector3 dir = Quaternion.Euler(0, 0, currentAngle) * Vector3.down;
        RaycastHit2D hit = Physics2D.Raycast(laserOrigin.position, dir, laserLength, LayerMask.GetMask("Terrain"));
        Vector3 laserEnd = hit.collider != null ? hit.point : laserOrigin.position + dir * laserLength;
        EventInstance laserBeamInstance = _audio.GetEventInstance(_audio.LaserBeam);
        laserBeamInstance.start();
        laserBeamInstance.set3DAttributes(RuntimeUtils.To3DAttributes(laserOrigin.position));
        if (sweepEffect != null)
        {
            sweepEffect.SetActive(true);
            sweepEffect.transform.position = laserEnd;
        }


        while ((endAngle > startAngle && currentAngle < endAngle) ||
               (endAngle < startAngle && currentAngle > endAngle))
        {
            float t = traveledAngle / totalAngle;
            float currentSpeed = Mathf.Lerp(startSpeed, endSpeed, t);
            dir = Quaternion.Euler(0, 0, currentAngle) * Vector3.down;
            hit = Physics2D.Raycast(laserOrigin.position, dir, laserLength, LayerMask.GetMask("Terrain"));
            laserEnd = hit.collider != null ? hit.point : laserOrigin.position + dir * laserLength;

            laserLine.SetPosition(0, laserOrigin.position);
            laserLine.SetPosition(1, laserEnd);

            laserBeamInstance.set3DAttributes(RuntimeUtils.To3DAttributes(laserEnd));
            sweepEffect.transform.position = laserEnd;

            if (!hasHitPlayer)
            {
                RaycastHit2D hitPlayer = Physics2D.Raycast(laserOrigin.position, dir, laserLength, LayerMask.GetMask("Hitbox_Player"));
                if (hitPlayer.collider != null)
                {
                    PlayerController playerController = hitPlayer.collider.GetComponentInParent<PlayerController>();
                    if (playerController != null && !playerController.GetPlayerInvincible() && playerController.IsAlive())
                    {
                        playerController.Hurt(10);
                        hasHitPlayer = true;
                    }
                }
            }
            float step = currentSpeed * Time.deltaTime;
            traveledAngle += step;
            currentAngle = Mathf.MoveTowards(currentAngle, endAngle, step);

            yield return null;
        }
        laserBeamInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        laserBeamInstance.release();
        sweepEffect.SetActive(false);
        laserLine.enabled = false;
    }

    private void AlternatingAttack()
    {
        if (Time.time > lastMissileTime + missileCooldown && isPlayerNearby)
        {
            bool isPathClear = !Physics2D.Linecast(turret_firePoint.position, player.position, LayerMask.GetMask("Terrain"));
            if (!isPathClear) return;

            lastMissileTime = Time.time;

            if (shouldFireMissile)
            {
                Instantiate(missile, turret_firePoint.position, turret_firePoint.rotation);
                _audio.PlayOneShot(_audio.MissileLaunch, turret_firePoint.position);
            }
            else
            {
                StartCoroutine(Zap());
            }

            shouldFireMissile = !shouldFireMissile;
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
        // Disable aim line
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
        // Disable sweep effect
        if (sweepEffect != null)
        {
            sweepEffect.SetActive(false);
        }
        // Disable laser line
        if (laserLine != null)
        {
            laserLine.enabled = false;
        }
        Vector3 explosionPos = transform.position;
        GameObject explosionInstance = Instantiate(explosion, explosionPos, Quaternion.identity);
        _audio.PlayOneShot(_audio.Explosion, transform.position);
        Destroy(explosionInstance.gameObject, 5f);
        base.Die(resourceAmount);
    }
}
