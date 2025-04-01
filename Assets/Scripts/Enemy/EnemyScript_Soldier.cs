using UnityEngine;
using System.Collections;

public class Enemy_Soldier : EnemyBase
{
    // Enemy movement
    public float moveSpeed;
    public float changeDirectionTime = 2f;
    private enum EnemyState { Patrol, Moving, Attack }
    private EnemyState currentState = EnemyState.Patrol;
    private bool movingLeft = true;
    private float directionTimer = 0f;
    private bool isIdle = false;

    // Enemy shooting
    [SerializeField] Transform turret;
    [SerializeField] Transform turret_firePoint;
    [SerializeField] GameObject turret_bullet;
    [SerializeField] ParticleSystem muzzleFlash_single;
    public float fireRate;
    public float detectionRange = 15f;
    private bool isShooting = false;
    private int shotsFired = 0;
    private int maxShots = 3;
    private float reloadTime = 1f;
    private float attackExitTime = 0f;
    private float attackToMoveDelay = 0.5f;
    private LineRenderer lineRenderer;
    private float stopAndShootDistance;

    private bool isPlayerNearby;
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioManager _audio;
    public Transform center;

    void Start()
    {
        _audio = AudioManager.Instance;
        isalive = true;
        resourceAmount = 320;
        maxHealth = 200;
        currentHealth = maxHealth;
        fireRate = 0.175f;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController != null)
        {
            player = playerController.GetAimPos();
        }
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        moveSpeed = 4f;
        animator = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        stopAndShootDistance = Random.Range(8f, 10f);
    }

    void Update()
    {
        if (_levelNumber >= 1)
        {
            maxShots = 5;
        }
        if (_levelNumber >= 2)
        {
            maxShots = 7;
        }
        if (isalive)
        {
            UpdateState();
            MoveSoldier();
            UpdateAim();
        }
    }

    private void LateUpdate()
    {
        if (isalive)
        {
            if (currentState == EnemyState.Attack)
            {
                Aim();
            }
        }
    }

    // Update states
    private void UpdateState()
    {
        isPlayerNearby = CheckNearbyPlayers();

        if (isShooting)
        {
            currentState = EnemyState.Attack;
            return;
        }
        if (isPlayerNearby)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > stopAndShootDistance)
            {
                if (currentState == EnemyState.Attack)
                {
                    if (Time.time > attackExitTime + attackToMoveDelay)
                    {
                        currentState = EnemyState.Moving;
                    }
                }
                else
                {
                    currentState = EnemyState.Moving;
                }
            }
            else
            {
                currentState = EnemyState.Attack;
                attackExitTime = Time.time;
            }
        }
        else
        {
            currentState = EnemyState.Patrol;
        }

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        if (currentState == EnemyState.Patrol)
        {
            animator.SetBool("isWalking", !isIdle);
            animator.SetBool("isIdle", isIdle);
            animator.SetBool("isRunning", false);
        }
        else if (currentState == EnemyState.Moving)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
            animator.SetBool("isRunning", true);
        }
        else if (currentState == EnemyState.Attack)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isIdle", true);
        }
    }

    // Move Solider
    private void MoveSoldier()
    {

        if (currentState == EnemyState.Patrol)
        {
            moveSpeed = 3f;
            Patrol();
        }
        else if (currentState == EnemyState.Moving)
        {
            moveSpeed = 8f;
            MoveToPlayer();
        }
        else if (currentState == EnemyState.Attack)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (player == null)
                return;

            float directionX = player.position.x - center.position.x;
            bool facingLeft = directionX < 0;
            UpdateSpriteDirection(facingLeft);
            bool isPathClear = !Physics2D.Linecast(turret_firePoint.position, player.position, LayerMask.GetMask("Terrain"));

            if (!isShooting && isPathClear)
            {
                StartCoroutine(ShootSequence());
            }
        }
    }


    private IEnumerator ShootSequence()
    {
        isShooting = true;
        yield return new WaitForSeconds(0.5f);

        while (shotsFired < maxShots)
        {
            if (!CheckNearbyPlayers())
            {
                isShooting = false;
                yield break;
            }
            Shoot();
            shotsFired++;
            yield return new WaitForSeconds(fireRate);
        }

        shotsFired = 0;
        if (isShooting)
        {
            yield return new WaitForSeconds(reloadTime);
        }
        isShooting = false;
    }
    private void UpdateAim()
    {
        if (CheckNearbyPlayers())
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, turret.position);
            lineRenderer.SetPosition(1, player.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void MoveToPlayer()
    {
        if (player == null)
            return;
        Vector2 direction = (player.position - transform.position).normalized;

        float moveDirectionX = 1f;

        if (direction.x < 0)
        {
            moveDirectionX = -1f;
        }

        rb.linearVelocity = new Vector2(moveDirectionX * moveSpeed, rb.linearVelocity.y);
        UpdateSpriteDirection(direction.x < 0);
    }


    private void Patrol()
    {
        if (isIdle)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        directionTimer += Time.deltaTime;
        if (directionTimer >= 2f)
        {
            StartCoroutine(IdleBeforeTurn());
        }
        else
        {
            float moveDirectionX = -1f;
            if (!movingLeft)
            {
                moveDirectionX = 1f;
            }

            rb.linearVelocity = new Vector2(moveDirectionX * moveSpeed, rb.linearVelocity.y);
            UpdateSpriteDirection(movingLeft);
        }
    }

    private void UpdateSpriteDirection(bool facingLeft)
    {
        if (facingLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private IEnumerator IdleBeforeTurn()
    {
        isIdle = true;
        UpdateAnimation();
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        movingLeft = !movingLeft;
        directionTimer = 0f;
        isIdle = false;
        UpdateAnimation();
    }

    private bool CheckNearbyPlayers()
    {
        if (player == null)
        {
            return false;
        }
        bool isPathClear = !Physics2D.Linecast(center.position, player.position, LayerMask.GetMask("Terrain"));
        return Vector3.Distance(center.position, player.position) <= detectionRange && isPathClear;
    }

    private void Aim()
    {
        if (isPlayerNearby)
        {
            Vector3 direction = (player.position - turret.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (transform.localScale.x < 0)
            {
                angle += 180f;
            }
            turret.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void Shoot()
    {
        if (turret_bullet != null && turret_firePoint != null)
        {
            _audio.SetParameterByName("WeaponType", 0);
            _audio.PlayOneShot(_audio.Shot, transform.position);
            Quaternion bulletRotation = turret_firePoint.rotation;
            if (transform.localScale.x < 0)
            {
                bulletRotation = Quaternion.Euler(0, 0, bulletRotation.eulerAngles.z + 180f);
            }
            muzzleFlash_single.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            muzzleFlash_single.Play();
            Instantiate(turret_bullet, turret_firePoint.position, bulletRotation);
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
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();

        foreach (var sprite in sprites)
        {
            sprite.color = Color.red;
        }

        yield return new WaitForSeconds(0.1f);

        foreach (var sprite in sprites)
        {
            sprite.color = Color.white;
        }
    }

    // Function that handles the enemy death
    protected override void Die(int amount)
    {
        isalive = false;
        lineRenderer.enabled = false;
        animator.SetTrigger("isDead");
        if (playerController != null)
        {
            playerController.OnEnemyKilled(resourceAmount);
        }

        StartCoroutine(WaitForDeathAnimation());
    }

    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(15f);
        base.Die(0);
    }

    public Transform getAimPos()
    {
        return center.transform;
    }
}
