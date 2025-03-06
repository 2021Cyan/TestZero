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
    public float fireRate;
    public float detectionRange = 15f;
    private bool isShooting = false; 
    private int shotsFired = 0; 
    private int maxShots = 5; 
    private float reloadTime = 1.5f; 


    private bool isPlayerNearby;
    private Transform player;
    private Rigidbody2D rb;
    private AudioManager _audio;

    void Start()
    {
        _audio = AudioManager.Instance;
        isalive = true;
        resourceAmount = 320;
        maxHealth = 200;
        currentHealth = maxHealth;
        fireRate = 0.175f;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player = playerController.GetAimPos();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1f;
        moveSpeed = 4f;
    }

    void Update()
    {
        UpdateState();
        MoveSoldier();
    }

    // Update states
    private void UpdateState()
    {
        isPlayerNearby = CheckNearbyPlayers();

        if (isPlayerNearby)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > 10f)
            {
                currentState = EnemyState.Moving;
            }
            else
            {
                currentState = EnemyState.Attack;
            }
        }
        else
        {
            currentState = EnemyState.Patrol;
        }
    }

    // Move Solider
    private void MoveSoldier()
    {
        if (currentState == EnemyState.Patrol)
        {
            moveSpeed = 4f;
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
            Aim();
            if (!isShooting)
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
            Shoot();
            shotsFired++;
            yield return new WaitForSeconds(fireRate);
        }

        shotsFired = 0;
        yield return new WaitForSeconds(reloadTime); 

        isShooting = false;
    }


    private void MoveToPlayer()
    {
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

    private IEnumerator IdleBeforeTurn()
    {
        isIdle = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(1f);
        movingLeft = !movingLeft;
        directionTimer = 0f;
        isIdle = false;
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
            turret.rotation = Quaternion.Euler(0, 0, angle + 45);
        }
    }

    private void Shoot()
    {
        if (turret_bullet != null && turret_firePoint != null)
        {
            _audio.PlayOneShot(_audio.Laser, transform.position);
            Instantiate(turret_bullet, turret_firePoint.position, turret_firePoint.rotation);
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
        base.Die(resourceAmount);
    }

    private void UpdateSpriteDirection(bool facingLeft)
    {
        if (facingLeft)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            turret.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
            turret.localScale = new Vector3(1, 1, 1);
        }
    }
}
