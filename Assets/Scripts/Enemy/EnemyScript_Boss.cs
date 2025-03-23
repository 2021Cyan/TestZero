using UnityEngine;
using System.Collections;

public class EnemyScript_Boss : EnemyBase
{

    // Floating effect
    private float floatStrength = 0.1f;
    private float floatSpeed = 5f;    
    private Vector3 initialPosition;

    // Boss shooting
    [SerializeField] GameObject turret_bullet;
    [SerializeField] Transform [] turrets;
    [SerializeField] Transform [] turret_firePoints;
    [SerializeField] LineRenderer [] lineRenderers;
    public float fireRate;
    private float lastFireTime = 0f;
    private int currentTurretIndex = 0;
    private bool isAiming = false;
    private bool isShooting = false;
    private bool isPatternRunning = false;

    // Enemy missile
    [SerializeField] Transform[] missile_firePoints;
    [SerializeField] GameObject missile;

    private delegate IEnumerator Pattern();
    private Pattern[] patternList;

    private bool isPlayerNearby;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioManager _audio;



    void Start()
    {
        _audio = AudioManager.Instance;
        isalive = true;
        resourceAmount = 0;
        maxHealth = 50000;
        currentHealth = maxHealth;
        fireRate = 0.05f;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        player = playerController.GetAimPos();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialPosition = transform.position;
        patternList = new Pattern[] { Pattern1, Pattern2};
        StartCoroutine(ManagePatterns());
    }

    void Update()
    {
        isPlayerNearby = CheckNearbyPlayers();
        ApplyFloatingEffect();

        if (isPatternRunning)
        {
            if (isAiming)
            {
                Aim();
            }
            else
            {
                ResetTurretRotation();
            }

            if (isShooting)
            {
                Shoot();
            }
        }

    }

    private IEnumerator ManagePatterns()
    {
        isPatternRunning = true;

        while (isalive)
        {
            int patternCount = patternList.Length;

            if (currentHealth <= maxHealth * 0.5f)
            {
                int indexA = Random.Range(0, patternCount);
                int indexB = Random.Range(0, patternCount);
                while (indexB == indexA) indexB = Random.Range(0, patternCount);
                yield return StartCoroutine(RunPatternsSimultaneously(patternList[indexA], patternList[indexB]));
            }
            else
            {
                int randomIndex = Random.Range(0, patternCount);
                yield return StartCoroutine(patternList[randomIndex]());
            }

            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator Pattern1()
    {
        isAiming = true;
        isShooting = false;
        yield return new WaitForSeconds(1f);

        isShooting = true;
        yield return new WaitForSeconds(5f);

        isAiming = false;
        isShooting = false;
    }

    private IEnumerator Pattern2()
    {
        for (int i = 0; i < missile_firePoints.Length; i++)
        {
            Transform firePoint = missile_firePoints[i];
            Instantiate(missile, firePoint.position, firePoint.rotation);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator RunPatternsSimultaneously(Pattern a, Pattern b)
    {
        Coroutine routineA = StartCoroutine(a());
        Coroutine routineB = StartCoroutine(b());
        yield return routineA;
        yield return routineB;
    }

    private void ApplyFloatingEffect()
    {
        float floatingY = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        transform.position = initialPosition + new Vector3(0, floatingY, 0);
    }


    private bool CheckNearbyPlayers()
    {
        if (player == null)
        {
            return false;
        }
        return Vector3.Distance(transform.position, player.position) <= 30f ;
    }

    private void Aim()
    {
        if (isPlayerNearby)
        {
            for (int i = 0; i < turrets.Length; i++)
            {
                if (lineRenderers.Length > i)
                {
                    lineRenderers[i].enabled = true;
                    lineRenderers[i].SetPosition(0, turret_firePoints[i].position);
                    lineRenderers[i].SetPosition(1, player.position);
                }

                Vector3 direction = (player.position - turrets[i].position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                turrets[i].rotation = Quaternion.Euler(0, 0, angle+90);
                turret_firePoints[i].rotation = Quaternion.Euler(0, 0, angle);
            }
        }
        else
        {
            foreach (var l in lineRenderers)
            {
                l.enabled = false;
            }
        }
    }

    private void Shoot()
    {
        if (!isPlayerNearby || Time.time < lastFireTime + fireRate)
        {
            return;
        }

        lastFireTime = Time.time;

        if (turret_bullet != null && turret_firePoints.Length > 0)
        {
            Transform firePoint = turret_firePoints[currentTurretIndex];

            _audio.PlayOneShot(_audio.Laser, firePoint.position);
            Instantiate(turret_bullet, firePoint.position, firePoint.rotation);

            currentTurretIndex++;
            if (currentTurretIndex >= turret_firePoints.Length)
            {
                currentTurretIndex = 0;
            }
        }
    }

    private void ResetTurretRotation()
    {
        for (int i = 0; i < turrets.Length; i++)
        {
            if (turrets[i] != null)
            {
                turrets[i].rotation = Quaternion.Euler(0, 0, 0);
            }
            if (turret_firePoints.Length > i && turret_firePoints[i] != null)
            {
                turret_firePoints[i].rotation = Quaternion.Euler(0, 0, 0);
            }
            if (lineRenderers.Length > i && lineRenderers[i] != null)
            {
                lineRenderers[i].enabled = false;
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
        base.Die(resourceAmount);
    }
}
